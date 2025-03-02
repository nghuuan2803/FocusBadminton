using Application.Features.Auth.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.Auth.Factories
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
                
                _ => throw new ArgumentException("Loại đăng nhập không hợp lệ.", nameof(loginType))
            };
        }
    }
}
