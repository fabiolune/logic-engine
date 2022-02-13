using System;
using System.Collections.Generic;
using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces
{
    public interface IRulesCompiler
    {
        IEnumerable<Func<T, Either<string, Unit>>> CompileRules<T>(IEnumerable<Rule> rules);
    }
}