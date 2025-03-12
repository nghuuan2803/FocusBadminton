using Domain.Common;
using Shared.Auth;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;

namespace Infrastructure.Identity.LoginStrategies
{
    public class GoogleLoginFlutterStrategy : ILoginStrategy
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public GoogleLoginFlutterStrategy(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string idToken)
        {
            var config = _configuration.GetSection("Authentication:Google");
            string clientId = config["ClientId"];   

            try
            {
                // Xác thực id_token trực tiếp
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });

                // Tìm hoặc tạo user dựa trên email từ payload
                var user = await _authService.FindOrCreateUserAsync(payload);

                // Tạo JWT token và refresh token
                string accessToken = await _authService.GenerateAccessToken(user);
                string refreshToken = _authService.GenerateRefreshToken();
                await _authService.SaveRefreshTokenAsync(user, refreshToken);

                return new AuthResponse(accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                return Error.Validation($"Xác thực id_token từ Flutter thất bại: {ex.Message}");
            }
        }
    }
}
