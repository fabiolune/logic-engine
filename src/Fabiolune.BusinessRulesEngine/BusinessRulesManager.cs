using System;
using System.Collections.Generic;
using System.Linq;
using Fabiolune.BusinessRulesEngine.Extensions;
using Fabiolune.BusinessRulesEngine.Interfaces;
using Fabiolune.BusinessRulesEngine.Models;

namespace Fabiolune.BusinessRulesEngine
{
    public class BusinessRulesManager<T> : IBusinessRulesManager<T> where T : new()
    {
        private readonly IBusinessRulesCompiler _businessRulesCompiler;
        private IEnumerable<IEnumerable<Func<T, RuleApplicationResult>>> _rulesCatalog;

        public BusinessRulesManager(IBusinessRulesCompiler businessRulesCompiler)
        {
            _businessRulesCompiler = businessRulesCompiler;
        }

        public void SetCatalog(RulesCatalog catalog)
        {
            _rulesCatalog = catalog
                .ToSimplifiedCatalog()
                .Select(x => _businessRulesCompiler.CompileRules<T>(x).ToArray()).ToArray();
        }

        /// <summary>
        ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
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

            // the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
            return _rulesCatalog
                .Select(ruleSet => ruleSet as Func<T, RuleApplicationResult>[])
                .Select(enumerable => enumerable.TakeWhile(rule => rule(item).Success)
                .Count() == enumerable.Length).Any(satisfiesSet => satisfiesSet);
        }
    }
}