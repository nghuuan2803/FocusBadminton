using Application.Features.Auth.Strategies;

namespace Application.Features.Auth.Factories
{
    public interface ILoginStrategyFactory
    {
        ILoginStrategy GetStrategy(string loginType);
    }
}
