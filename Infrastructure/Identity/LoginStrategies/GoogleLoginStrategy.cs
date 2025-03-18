using Shared.Auth;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Infrastructure.Data;

namespace Infrastructure.Identity.LoginStrategies
{
    public class GoogleLoginStrategy : ILoginStrategy
    {
        private readonly IAuthService _authService;
        private readonly UserManager<Account> _userManager;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public GoogleLoginStrategy(IAuthService authService, 
            UserManager<Account> userManager,
            IConfiguration configuration, 
            HttpClient httpClient, 
            AppDbContext dbContext)
        {
            _authService = authService;
            _userManager = userManager;
            _configuration = configuration;
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string authCode)
        {
            // Auth code
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
                    //tạo member mới
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
                    await _userManager.AddToRoleAsync(user, "member");
                }

                // Bước 4: Tạo access token và refresh token
                string accessToken = await _authService.GenerateAccessToken(user);
                string refreshToken = _authService.GenerateRefreshToken();
                await _authService.SaveRefreshTokenAsync(user, refreshToken);

                return new AuthResponse(accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                return Error.Validation($"Xác thực id_token thất bại: {ex.Message}");
            }
        }
    }
}
