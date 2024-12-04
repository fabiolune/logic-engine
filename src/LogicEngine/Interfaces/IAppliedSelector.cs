using TinyFp;

namespace LogicEngine.Interfaces;

public interface IAppliedSelector<TIn, TOut> where TIn : new()
{
    /// <summary>
    /// Returns the first matching item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Option<TOut> FirstMatching(TIn item);
}