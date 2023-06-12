using TinyFp;

namespace LogicEngine.Interfaces;

public interface IDetailedApplyable<T, TOut> where T : new()
{
    Either<TOut, Unit> DetailedApply(T item);
}