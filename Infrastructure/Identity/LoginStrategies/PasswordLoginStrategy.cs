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
            string email = info[0];
            string password = info[1];
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return Error.Validation("Email hoặc mật khẩu không đúng.");
            }

            string accessToken = await _authService.GenerateAccessToken(user);
            string refreshToken = _authService.GenerateRefreshToken();
            await _authService.SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponse(accessToken, refreshToken);
        }
    }
}