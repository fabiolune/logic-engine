using System;
using System.Collections.Generic;
using System.Linq;
using RulesEngine.Extensions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using static TinyFp.Prelude;
using TinyFp;
using TinyFp.Extensions;
using static System.Array;

namespace RulesEngine
{
    public class RulesManager<T> : IRulesManager<T> where T : new()
    {
        private readonly IRulesCompiler _rulesCompiler;
        private IEnumerable<IEnumerable<Func<T, Either<Option<string>, Unit>>>> _rulesCatalog;

        public RulesManager(IRulesCompiler rulesCompiler)
        {
            _rulesCompiler = rulesCompiler;
        }

        public void SetCatalog(RulesCatalog catalog)
            => _rulesCatalog = catalog
                .ToSimplifiedCatalog()
                .Select(x => _rulesCompiler.CompileRules<T>(x).ToArray()).ToArray();

        /// <summary>
        ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR);
        ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>RulesCatalogApplicationResult</returns>
        public Either<IEnumerable<Option<string>>, Unit> ItemSatisfiesRulesWithMessage(T item) =>
            (_rulesCatalog, Empty<Option<string>>())
            .Map(_ => Loop(_, item))
            .Bind(_ => _.Item2.ToOption(x => !x.Any()))
            .Map(_ => _.Distinct())
            .Match(Either<IEnumerable<Option<string>>, Unit>.Left, () => Unit.Default);

        private static Option<(IEnumerable<IEnumerable<Func<T, Either<Option<string>, Unit>>>>, IEnumerable<Option<string>>)> Loop(
            (IEnumerable<IEnumerable<Func<T, Either<Option<string>, Unit>>>>, IEnumerable<Option<string>>) data, T item) =>
            data
                .ToOption(_ => !_.Item1.Any() && !_.Item2.Any())
                .Bind(_ => _.Map(r => !r.Item1.Any()
                    ? Some(r)
                    : r
                        .Item1
                        .First()
                        .Select(x => x.Invoke(item))
                        .Where(x => !x.IsRight)
                        .ToOption(x => x.All(y => y.IsRight))
                        .Bind(l => Loop((r.Item1.Skip(1),
                                r.Item2.Concat(l.Where(x => x.IsLeft)
                                    .Select(x => x.Match(y => Option<string>.None(), y => y)))), item))));

        /// <summary>
        ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
        ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>bool</returns>
        public bool ItemSatisfiesRules(T item) =>
            _rulesCatalog
                .ToOption(_ => !_.Any())
                .Match(_ => _.ToArray()
                    .Select(ruleSet => ruleSet as Func<T, Either<Option<string>, Unit>>[])
                    .Select(e => e.TakeWhile(rule => rule(item).IsRight).Count() == e.Length)
                    .Any(s => s), () => true);

        /// <summary>
        ///     It filters the items keeping only those that satisfy the rules
        /// </summary>
        /// <param name="items"></param>
        /// <returns><![CDATA[IEnumerable<T>]]></returns>
        public IEnumerable<T> Filter(IEnumerable<T> items)
            => items.Where(ItemSatisfiesRules);

        /// <summary>
        ///     Returns the first item that matches the condition
        /// </summary>
        /// <param name="items"></param>
        /// <returns>T</returns>
        public T FirstOrDefault(IEnumerable<T> items)
            => items.FirstOrDefault(ItemSatisfiesRules);
    }
}