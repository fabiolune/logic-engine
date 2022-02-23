using System;
using System.Collections.Generic;
using TinyFp;

namespace LogicEngine.Models;

public record CompiledLabeledRulesSet<T>
{
    public Option<KeyValuePair<string, Func<T, Either<string, Unit>>>[]> Executables { get; init; }

    public CompiledLabeledRulesSet(Option<KeyValuePair<string, Func<T, Either<string, Unit>>>[]> executables)
    {
        Executables = executables;
    }
}