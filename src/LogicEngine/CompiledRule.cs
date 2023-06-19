using LogicEngine.Interfaces;
using System;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine;

public record CompiledRule<T> : IAppliable<T>, IDetailedAppliable<T, string> where T : new()
{
    public string Code { get; }
    private readonly Func<T, bool> _executable;

    public CompiledRule(Func<T, bool> executable, string code) =>
        (_executable, Code) = (executable, code);

    public bool Apply(T item) => _executable(item);

    public Either<string, Unit> DetailedApply(T item) => _executable(item).ToEither(_ => Unit.Default, b => !b, Code);
}