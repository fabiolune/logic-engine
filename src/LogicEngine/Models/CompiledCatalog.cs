using System;
using System.Diagnostics.CodeAnalysis;
using TinyFp;

namespace LogicEngine.Models;

[ExcludeFromCodeCoverage]
public record CompiledCatalog<T>
{
    public Func<T, Either<string, Unit>>[][] Executables { get; init; }

    public CompiledCatalog(Func<T, Either<string, Unit>>[][] executables)
    {
        Executables = executables;
    }
}