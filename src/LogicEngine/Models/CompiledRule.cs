using System;
using TinyFp;

namespace LogicEngine.Models;

public record CompiledRule<T>
{
    public CompiledRule(Func<T, Either<string, Unit>> executable) => Executable = executable;

    public Func<T, Either<string, Unit>> Executable { get; init; }
}