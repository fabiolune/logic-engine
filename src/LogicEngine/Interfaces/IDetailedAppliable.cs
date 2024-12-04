using TinyFp;

namespace LogicEngine.Interfaces;

public interface IDetailedAppliable<T, TOut> where T : new()
{
    /// <summary>
    /// Applies the item to the appliable
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Either<TOut, Unit> DetailedApply(T item);
}