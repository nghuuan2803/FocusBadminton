using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.LoginStrategies
{
    public class LoginStrategyFactory : ILoginStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public LoginStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILoginStrategy GetStrategy(string loginType)
        {
            return loginType.ToLower() switch
            {
                "google" => _serviceProvider.GetRequiredService<GoogleLoginStrategy>(),
                "google-flutter" => _serviceProvider.GetRequiredService<GoogleLoginFlutterStrategy>(),
                "facebook" => _serviceProvider.GetRequiredService<FacebookLoginStrategy>(),
                "password" => _serviceProvider.GetRequiredService<PasswordLoginStrategy>(),
                _ => throw new ArgumentException("Loại đăng nhập không hợp lệ.", nameof(loginType))
            };
        }
    }
}
