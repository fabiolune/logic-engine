using TinyFp;

namespace LogicEngine.Interfaces;

public interface IDetailedAppliable<T, TOut> where T : new()
{
    Either<TOut, Unit> DetailedApply(T item);
}