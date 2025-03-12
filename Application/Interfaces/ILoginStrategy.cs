using Shared.Auth;

namespace Application.Interfaces
{
    public interface ILoginStrategy
    {
        // Credential với google là : Authcode
        // ....       với password là : email / password
        Task<Result<AuthResponse>> LoginAsync(string credential);
    }
}
