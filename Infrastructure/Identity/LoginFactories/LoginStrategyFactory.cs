using Application.Interfaces;
using Infrastructure.Identity.LoginStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.LoginFactories
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
                "google-flutter" => _serviceProvider.GetRequiredService<MobileGoogleLoginStrategy>(),
                "facebook" => _serviceProvider.GetRequiredService<FacebookLoginStrategy>(),
                "password" => _serviceProvider.GetRequiredService<PasswordLoginStrategy>(),
                _ => throw new ArgumentException("Loại đăng nhập không hợp lệ.", nameof(loginType))
            };
        }
    }
}
