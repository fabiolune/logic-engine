using System;
using System.Collections.Generic;
using System.Linq;
using RulesEngine.Extensions;
using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace RulesEngine
{
    public class RulesManager<T> : IRulesManager<T> where T : new()
    {
        private readonly IRulesCompiler _rulesCompiler;
        private IEnumerable<IEnumerable<Func<T, RuleApplicationResult>>> _rulesCatalog;

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
        public RulesCatalogApplicationResult ItemSatisfiesRulesWithMessage(T item)
        {
            if (!_rulesCatalog.Any())
                return RulesCatalogApplicationResult.Successful;

            var codes = new List<string>();
            foreach (var ruleSet in _rulesCatalog)
            {
                var rulesApplicationResult = ruleSet.ToDictionary(r => r, r => r(item));
                if (rulesApplicationResult.All(x => x.Value.Success))
                    return RulesCatalogApplicationResult.Successful;

                var failingReasons = rulesApplicationResult
                    .Where(r => !r.Value.Success)
                    .Select(r => r.Value.Code)
                    .Distinct();
                codes.AddRange(failingReasons);
            }
            return RulesCatalogApplicationResult.Failed(codes.Distinct());
        }

        /// <summary>
        ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
        ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>bool</returns>
        public bool ItemSatisfiesRules(T item)
        {
            if (!_rulesCatalog.Any())
                return true;

            return _rulesCatalog.ToArray()
                .Select(ruleSet => ruleSet as Func<T, RuleApplicationResult>[])
                .Select(enumerable => enumerable.TakeWhile(rule => rule(item).Success).Count() == enumerable.Length)
                .Any(satisfiesSet => satisfiesSet);
        }

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