using TinyFp;

namespace LogicEngine.Interfaces;

internal interface IApplyable<TIn, TOut> where TIn : new()
{
    bool Apply(TIn item);
    Either<TOut, Unit> DetailedApply(TIn item);
}
