using System.Collections.Generic;
using RulesEngine.Interfaces;
using TinyFp;

namespace RulesEngine.Extensions
{
    public static class Extensions
    {
        public static bool SatisfiesRules<T>(this T @this, IRulesManager<T> manager) where T : new() =>
            manager.ItemSatisfiesRules(@this);

        public static Either<IEnumerable<Option<string>>, Unit> SatisfiesRulesWithMessage<T>(this T @this, IRulesManager<T> manager) where T : new() =>
            manager.ItemSatisfiesRulesWithMessage(@this);
    }
}