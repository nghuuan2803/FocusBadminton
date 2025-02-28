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
            switch (loginType.ToLower())
            {
                case "google":
                    return _serviceProvider.GetRequiredService<GoogleLoginStrategy>();
                // Bạn có thể thêm các case khác như "facebook", "password", v.v.
                default:
                    throw new ArgumentException("Loại đăng nhập không hợp lệ.", nameof(loginType));
            }
        }
    }
}
