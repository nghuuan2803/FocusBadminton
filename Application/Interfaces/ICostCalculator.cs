using Shared.CostCalculators;

namespace Application.Interfaces
{
    public interface ICostCalculator
    {
        Task<double> CalculateAsync(CostCalculatorRequest request);
    }
}
