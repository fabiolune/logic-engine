using System;
using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Models;

namespace Fabiolune.BusinessRulesEngine.Interfaces
{
    public interface IBusinessRulesCompiler
    {
        IEnumerable<Func<T, RuleApplicationResult>> CompileRules<T>(IEnumerable<Rule> rules);
    }
}