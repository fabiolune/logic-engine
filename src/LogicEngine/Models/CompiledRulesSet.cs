using System;
using TinyFp;

namespace LogicEngine.Models;

public record CompiledRulesSet<T>
{
    public Func<T, Either<string, Unit>>[] Executables { get; init; }

    public CompiledRulesSet(Func<T, Either<string, Unit>>[] executables)
    {
        Executables = executables;
    }
}