using Shared.Auth;

namespace Application.Interfaces
{
    public interface ILoginStrategy
    {
        // Credential với đăng nhập google trên web là : Authcode
        // Credential với đăng nhập google trên app là : idToken
        // ....       với đăng nhập bằng tài khoản hệ thống là : email|password
        Task<Result<AuthResponse>> LoginAsync(string credential);
    }
}
