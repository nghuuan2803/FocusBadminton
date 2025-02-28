using Shared.Auth;

namespace Sh.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginByPasswordAsync(string email, string password);
        Task<Result<AuthResponse>> LoginByGoogleAsync(string authCode);
        Task<Result<AuthResponse>> LoginByFaceBookAsync(string idToken);
        Task<Result<AuthResponse>> RefreshTokensAsync(string refreshToken);
        Task<Result> LogoutAsync(string email);
    }
}
