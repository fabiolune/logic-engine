using TinyFp;

namespace LogicEngine.Interfaces;

public interface IAppliedSelector<TIn, TOut> where TIn : new()
{
    Option<TOut> FirstMatching(TIn item);
}