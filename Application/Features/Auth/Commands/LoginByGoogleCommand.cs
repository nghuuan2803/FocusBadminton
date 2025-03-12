/*using Application.Interfaces;
using Shared.Auth;

namespace Application.Features.Auth.Commands
{
    public record LoginByGoogleCommand(string Code) : IRequest<Result<AuthResponse>>;

    public class LoginByGoogleCommandHandler : IRequestHandler<LoginByGoogleCommand, Result<AuthResponse>>
    {
        private readonly IAuthService _authService;
        public LoginByGoogleCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginByGoogleCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginByGoogleAsync(request.Code);
        }
    }
}
*/