using Google.Apis.Auth;
using Shared.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginByPasswordAsync(string email, string password);
        Task<Result<AuthResponse>> LoginByGoogleAsync(string authCode);
        Task<Result<AuthResponse>> LoginByFacebookAsync(string accessToken);
        Task<Result<AuthResponse>> RefreshTokensAsync(string refreshToken);
        Task<Account> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
        Task<Account> FindOrCreateUserByPhoneAsync(string phoneNumber, string password);
        Task<string> GenerateAccessToken(Account user);
        string GenerateRefreshToken();
        Task SaveRefreshTokenAsync(Account user, string refreshToken);
    }
}
