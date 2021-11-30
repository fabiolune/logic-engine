using System;
using System.Collections.Generic;
using RulesEngine.Models;

namespace RulesEngine.Interfaces
{
    public interface IRulesCompiler
    {
        IEnumerable<Func<T, RuleApplicationResult>> CompileRules<T>(IEnumerable<Rule> rules);
    }
}