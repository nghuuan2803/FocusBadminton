using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.Auth;

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
            return Error.Validation("Credential không hợp lệ. Định dạng: identifier|password");
        }

        string identifier = info[0]; // Có thể là email hoặc số điện thoại
        string password = info[1];

        // Tìm hoặc tạo user bằng identifier
        var user = await _authService.FindOrCreateUserByIdentifierAsync(identifier, password);

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