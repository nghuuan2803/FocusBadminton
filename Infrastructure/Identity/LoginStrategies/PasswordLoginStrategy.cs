using Shared.Auth;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.LoginStrategies
{
    public class PasswordLoginStrategy : ILoginStrategy
    {
        private readonly IAuthService _authService;
        private readonly UserManager<Account> _userManager;

        public PasswordLoginStrategy(IAuthService authService, UserManager<Account> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string credential)
        {
            var info = credential.Split('|');
            if (info.Length != 2)
            {
                return Error.Validation("Credential không hợp lệ. Định dạng: phoneNumber|password");
            }

            string phoneNumber = info[0];
            string password = info[1];

            // Tìm hoặc tạo user
            var user = await _authService.FindOrCreateUserByPhoneAsync(phoneNumber, password);

            // Kiểm tra mật khẩu
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return Error.Validation("Mật khẩu không đúng.");
            }

            string accessToken = await _authService.GenerateAccessToken(user);
            string refreshToken = _authService.GenerateRefreshToken();

            await _authService.SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponse(accessToken, refreshToken);
        }
    }
}