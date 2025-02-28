using Shared.Auth;
using Application.Interfaces;

namespace Application.Features.Auth.Strategies
{
    public class GoogleLoginStrategy : ILoginStrategy
    {
        private readonly IAuthService _authService;
        public GoogleLoginStrategy(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string credential)
        {
            return await _authService.LoginByGoogleAsync(credential);
        }
    }
}
