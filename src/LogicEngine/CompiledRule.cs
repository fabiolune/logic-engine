using LogicEngine.Interfaces;
using System;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine;

/* The code defines a public record called `CompiledRule<T>`. This record implements two interfaces:
`IAppliable<T>` and `IDetailedAppliable<T, string>`. The `CompiledRule<T>` record has a generic type
parameter `T` which must have a parameterless constructor. */
public record CompiledRule<T> : IAppliable<T>, IDetailedAppliable<T, string> where T : new()
{
    /// <summary>
    /// The code that represents the rule
    /// </summary>
    public string Code { get; }
    private readonly Func<T, bool> _executable;

    /// <summary>
    /// Creates a new compiled rule
    /// </summary>
    /// <param name="executable"></param>
    /// <param name="code"></param>
    public CompiledRule(Func<T, bool> executable, string code) =>
        (_executable, Code) = (executable, code);

    /// <summary>
    /// Applies the rule to an item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Apply(T item) => _executable(item);

    /// <summary>
    /// Applies the rule to an item and returns either a string (the code if the rule is not satified) or a unit
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Either<string, Unit> DetailedApply(T item) => _executable(item).ToEither(_ => Unit.Default, b => !b, Code);
}