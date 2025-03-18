using Shared.CostCalculators;

namespace Application.Interfaces
{
    public interface ICostCalculatorFactory
    {
        ICostCalculator CreateCalculator(CostCalculatorRequest request); // Factory Method
    }
}
