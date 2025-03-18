namespace Domain.Interfaces
{
    public interface IClone<T> where T : class
    {
        T Clone();
    }
}
