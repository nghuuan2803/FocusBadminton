namespace Application.Interfaces
{
    public interface ILoginStrategyFactory
    {
        ILoginStrategy GetStrategy(string loginType);
    }
}
