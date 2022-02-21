using LogicEngine.Interfaces;
using TinyFp;

namespace LogicEngine.Extensions;

public static class Extensions
{
    public static bool SatisfiesRules<T>(this T @this, IRulesManager<T> manager) where T : new() =>
        manager.ItemSatisfiesRules(@this);

    public static Either<string[], Unit> SatisfiesRulesWithMessage<T>(this T @this, IRulesManager<T> manager) where T : new() =>
        manager.ItemSatisfiesRulesWithMessage(@this);
}