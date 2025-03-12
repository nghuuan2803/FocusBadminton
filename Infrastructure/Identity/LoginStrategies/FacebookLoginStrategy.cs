using Shared.Auth;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Application.Interfaces;

namespace Infrastructure.Identity.LoginStrategies
{
    public class FacebookLoginStrategy : ILoginStrategy
    {
        private readonly IAuthService _authService;
        private readonly UserManager<Account> _userManager;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public FacebookLoginStrategy(IAuthService authService, 
            UserManager<Account> userManager, 
            IConfiguration configuration, 
            HttpClient httpClient,
            AppDbContext dbContext)
        {
            _authService = authService;
            _userManager = userManager;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string accessToken)
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
                string accessTokenJwt = await _authService.GenerateAccessToken(user);
                string refreshToken = _authService.GenerateRefreshToken();
                await _authService.SaveRefreshTokenAsync(user, refreshToken);

                return new AuthResponse(accessTokenJwt, refreshToken);
            }
            catch (Exception ex)
            {
                return Error.Validation($"Đăng nhập bằng Facebook thất bại: {ex.Message}");
            }
        }
    }
}