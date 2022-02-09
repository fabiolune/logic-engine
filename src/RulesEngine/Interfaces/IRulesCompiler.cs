using System;
using System.Collections.Generic;
using RulesEngine.Models;
using TinyFp;

namespace RulesEngine.Interfaces
{
    public interface IRulesCompiler
    {
        IEnumerable<Func<T, Either<string, Unit>>> CompileRules<T>(IEnumerable<Rule> rules);
    }
}