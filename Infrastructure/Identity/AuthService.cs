using Domain.Common;
﻿using Google.Apis.Auth;
using System.Text.Json;
using Domain.Entities;
using Shared.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


public class AuthService : IAuthService
{
    private readonly UserManager<Account> _userManager;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;

    public AuthService(AppDbContext dbContext,UserManager<Account> userManager, IConfiguration configuration, HttpClient httpClient)
    {
        _userManager = userManager;
        _configuration = configuration;
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    public async Task<Result<AuthResponse>> LoginByGoogleAsync(string authCode)
    {
        var config = _configuration.GetSection("Authentication:Google");
        string clientId = config["ClientId"];
        string clientSecret = config["ClientSecret"];
        string redirectUri = "https://localhost:7000/google-callback"; // Phải khớp với redirectUri đã đăng ký

        // Bước 1: Trao đổi auth code lấy token từ Google
        var tokenRequestParams = new Dictionary<string, string>
        {
            { "code", authCode },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        };

        var requestContent = new FormUrlEncodedContent(tokenRequestParams);
        var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            return Error.Validation($"Trao đổi auth code thất bại. Status code: {(int)tokenResponse.StatusCode}");
        }

        var jsonResponse = await tokenResponse.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonResponse);
        if (!jsonDoc.RootElement.TryGetProperty("id_token", out var idTokenElement))
        {
            return Error.Validation("Không tìm thấy id_token trong phản hồi của Google.");
        }
        var idToken = idTokenElement.GetString();
        if (string.IsNullOrEmpty(idToken))
        {
            return Error.Validation("id_token rỗng.");
        }

        // Bước 2: Xác thực id_token với Google
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            });

            // Bước 3: Tìm hoặc tạo user dựa trên email từ payload
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return Error.Validation("Tạo người dùng mới thất bại.");
                }

                var member = new Member
                {
                    AccountId = user.Id,
                    FullName = payload.Name,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "System",
                    Email = payload.Email,
                };
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();

                // Gán role mặc định nếu cần
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            // Bước 4: Tạo access token và refresh token
            string accessToken = await GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponse(accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            return Error.Validation($"Xác thực id_token thất bại: {ex.Message}");
        }
    }

    // Tạo JWT token 
    public async Task<string> GenerateAccessToken(Account user)
    { 
        string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.AccountId == user.Id);

        var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                //new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Role,role),
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Dùng Id thay vì UserName
            };

        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var secureToken = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:Expire"])),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
            );

        return new JwtSecurityTokenHandler().WriteToken(secureToken);
    }

    // Tạo refresh token ngẫu nhiên
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    // Lưu refresh token vào DB
    public async Task SaveRefreshTokenAsync(Account user, string refreshToken)
    {
        await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
    }

    public async Task<Result<AuthResponse>> LoginByFacebookAsync(string accessToken)
    {
        try
        {
            // Xác thực access token với Facebook
            var appId = _configuration["Authentication:Facebook:AppId"];
            var appSecret = _configuration["Authentication:Facebook:AppSecret"];
            var verifyUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";

            var response = await _httpClient.GetAsync(verifyUrl);
            if (!response.IsSuccessStatusCode)
            {
                return Error.Validation("Xác thực access token thất bại.");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(jsonResponse);
            if (!jsonDoc.RootElement.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("is_valid", out var isValid) || !isValid.GetBoolean())
            {
                return Error.Validation("Access token không hợp lệ.");
            }

            // Lấy thông tin người dùng từ Facebook
            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}";
            var userInfoResponse = await _httpClient.GetAsync(userInfoUrl);
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                return Error.Validation("Lấy thông tin người dùng thất bại.");
            }

            var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
            using var userInfoDoc = JsonDocument.Parse(userInfoJson);
            var email = userInfoDoc.RootElement.GetProperty("email").GetString();
            var name = userInfoDoc.RootElement.GetProperty("name").GetString();

            // Tìm hoặc tạo user dựa trên email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return Error.Validation("Tạo người dùng mới thất bại.");
                }

                var member = new Member
                {
                    AccountId = user.Id,
                    FullName = name,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "System",
                    Email = email,
                };
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "Customer");
            }

            // Tạo access token và refresh token
            string accessTokenJwt = await GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponse(accessTokenJwt, refreshToken);
        }
        catch (Exception ex)
        {
            return Error.Validation($"Đăng nhập bằng Facebook thất bại: {ex.Message}");
        }
    }
    public async Task<Result<AuthResponse>> LoginByPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Validation("Email hoặc mật khẩu không đúng.");
        }

        string accessToken = await GenerateAccessToken(user);
        string refreshToken = GenerateRefreshToken();
        await SaveRefreshTokenAsync(user, refreshToken);

        return new AuthResponse(accessToken, refreshToken);
    }
    public Task<Result> LogoutAsync(string email) => Task.FromResult(Result.Failure(Error.Validation("Not Implemented")));
    public Task<Result<AuthResponse>> RefreshTokensAsync(string refreshToken) => Task.FromResult(Result<AuthResponse>.Failure(Error.Validation("Not Implemented")));

    public async Task<Account> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        //var user = await _userManager.FindByEmailAsync(payload.Email);
        var user = await _dbContext.Users.FirstOrDefaultAsync(p=>p.Email == payload.Email);
        if (user == null)
        {
            user = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserName = payload.Email,
                Email = payload.Email,
                EmailConfirmed = true,
                Avatar = payload.Picture ?? string.Empty,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "Google"
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new Exception("Tạo người dùng mới thất bại.");
            }

            var member = new Member
            {
                AccountId = user.Id,
                FullName = payload.Name ?? string.Empty,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "Google"
            };
            await _dbContext.Members.AddAsync(member);
            await _dbContext.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, "member");
        }
        return user;
    }

    // Phương thức mới cho Password Sign-In với số điện thoại

    public async Task<Account> FindOrCreateUserByPhoneAsync(string phoneNumber, string password)
    {
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.PhoneNumber == phoneNumber);

        Account user;
        if (member != null)
        {
            user = await _userManager.FindByIdAsync(member.AccountId);
            if (user == null)
            {
                throw new Exception($"Không tìm thấy Account cho Member với PhoneNumber: {phoneNumber}");
            }
        }
        else
        {
            user = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserName = phoneNumber, // Dùng phoneNumber làm UserName
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "Password"
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new Exception("Tạo người dùng mới thất bại: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            member = new Member
            {
                AccountId = user.Id,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "Password"
                // Nếu muốn FullName hoặc Email, cần thêm vào credential hoặc yêu cầu riêng
            };
            await _dbContext.Members.AddAsync(member);
            var rowsAffected = await _dbContext.SaveChangesAsync();
            Console.WriteLine($"FindOrCreate - Rows affected: {rowsAffected}, AccountId: {member.AccountId}, PhoneNumber: {member.PhoneNumber}");

            await _userManager.AddToRoleAsync(user, "member");
        }

        return user;
    }
    public async Task<Account> FindOrCreateUserByIdentifierAsync(string identifier, string password)
    {
        // Kiểm tra xem identifier là email hay số điện thoại
        bool isEmail = identifier.Contains("@");
        Account user;

        if (isEmail)
        {
            user = await _userManager.FindByEmailAsync(identifier);
            if (user == null)
            {
                // Tạo mới user nếu không tồn tại
                user = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = identifier,
                    Email = identifier,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "Password"
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new Exception("Tạo người dùng mới thất bại: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                var member = new Member
                {
                    AccountId = user.Id,
                    Email = identifier,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "Password"
                };
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "member");
            }
        }
        else // Giả sử là số điện thoại
        {
            var member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.PhoneNumber == identifier);

            if (member != null)
            {
                user = await _userManager.FindByIdAsync(member.AccountId);
                if (user == null)
                {
                    throw new Exception($"Không tìm thấy Account cho Member với PhoneNumber: {identifier}");
                }
            }
            else
            {
                user = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = identifier,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "Password"
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new Exception("Tạo người dùng mới thất bại: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                member = new Member
                {
                    AccountId = user.Id,
                    PhoneNumber = identifier,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "Password"
                };
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "member");
            }
        }

        return user;
    }
}
