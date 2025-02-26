using Domain.Common;
using Domain.Entities;
using Google.Apis.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Shared.Auth;
using Application.Interfaces;

namespace Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private List<Error> errors = new();
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public AuthService(UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            AppDbContext dbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<Result<AuthResponse>> LoginByPasswordAsync(string email, string password)
        {
            Account authUser = (await _userManager.FindByEmailAsync(email))!;

            if (authUser == null)
            {
                errors.Add(Error.Unauthorized("Tài khoản không tồn tại!"));
                return Result<AuthResponse>.Failure(errors);
            }
            // so mật khẩu
            var loginResult = await _signInManager.PasswordSignInAsync(authUser, password, false, false);
            if (!loginResult.Succeeded)
            {
                errors.Add(Error.Unauthorized("Mật khẩu không chính xác!"));
                return Result<AuthResponse>.Failure(errors);
            }

            string accessToken = await GenerateAccessToken(authUser);
            string? refreshToken = GenerateRefreshToken();
            AuthResponse AuthResponse = new(accessToken, refreshToken);
            return Result<AuthResponse>.Success(AuthResponse);
        }

        public async Task<Result<AuthResponse>> LoginByGoogleAsync(string idToken)
        {
            var config = _configuration.GetSection("Authentication:Google");
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { config["ClientId"] }
            });
            var user = await FindOrCreateUserAsync(payload);
            string accessToken = await GenerateAccessToken(user);
            string? refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, refreshToken);
            AuthResponse AuthResponse = new(accessToken, refreshToken);
            return Result<AuthResponse>.Success(AuthResponse);
        }

        public async Task<Result<AuthResponse>> RefreshTokensAsync(string refreshToken)
        {
            var userToken = await _dbContext.UserTokens.FirstOrDefaultAsync(p => p.Value == refreshToken);
            if (userToken == null)
            {
                return Result<AuthResponse>.Failure(Error.Unauthorized("RefreshToken không hợp lệ"));
            }
            var user = await _userManager.FindByIdAsync(userToken.UserId.ToString());
            if (user == null)
            {
                return Result<AuthResponse>.Failure(Error.Unauthorized("Tài khoản không tồn tại"));
            }

            var accessToken = await GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, newRefreshToken);
            AuthResponse AuthResponse = new(accessToken, newRefreshToken);
            return Result<AuthResponse>.Success(AuthResponse);
        }

        private async Task SaveRefreshTokenAsync(Account user, string refreshToken)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private async Task<string> GenerateAccessToken(Account user)
        {
            string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.UserName!),
                new Claim(ClaimTypes.Role,role),
            };

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var secureToken = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:Expire"]!)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(secureToken);
        }

        private async Task<Account> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
        {
            // Tìm người dùng theo email, hoặc tạo người dùng mới nếu chưa tồn tại
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = payload.Email,
                    UserName = payload.Email,
                    //FullName = payload.Name ?? string.Empty,
                    Avatar = payload.Picture,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = "Google"
                };

                await _userManager.CreateAsync(user);

                // Gán role cho người dùng
                await _userManager.AddToRoleAsync(user, "Customer");
                // tạo member
                var member = new Member
                {
                    FullName = payload.Email,
                    CreatedAt = DateTimeOffset.Now,
                    PhoneNumber = string.Empty,
                    AccountId = user.Id,
                };
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();
            }
            return user;
        }

        public async Task<Result> LogoutAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
                return Result.Success();
            }
            return Result.Failure(Error.NotFound("Account", email));
        }

        public Task<Result<AuthResponse>> LoginByFaceBookAsync(string idToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveTokenAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}