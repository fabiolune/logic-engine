using LogicEngine.Interfaces;
using System;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine;

/// <summary>
/// Represents a compiled rule that can be applied to an object of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>This class provides functionality to evaluate a rule against an object of type <typeparamref
/// name="T"/>  and optionally retrieve detailed information about the evaluation result.</remarks>
/// <typeparam name="T">The type of object to which the rule can be applied. Must have a parameterless constructor.</typeparam>
public record CompiledRule<T> : IAppliable<T>, IDetailedAppliable<T, string> where T : new()
{
    /// <summary>
    /// The code that represents the rule
    /// </summary>
    public string Code { get; }
    private readonly Func<T, bool> _executable;

    /// <summary>
    /// Represents a compiled rule that can be executed against a specified input.
    /// </summary>
    /// <param name="executable">A function that defines the rule logic and determines whether the input satisfies the rule.</param>
    /// <param name="code">A string representing the unique code or identifier for the rule.</param>
    public CompiledRule(Func<T, bool> executable, string code) =>
        (_executable, Code) = (executable, code);

    /// <summary>
    /// Applies the specified condition or operation to the given item.
    /// </summary>
    /// <param name="item">The item to which the condition or operation is applied.</param>
    /// <returns><see langword="true"/> if the condition or operation succeeds; otherwise, <see langword="false"/>. </returns>
    public bool Apply(T item) => _executable(item);

    /// <summary>
    /// Applies the specified operation to the given item and returns the result as an <see cref="Either{TLeft,
    /// TRight}"/>.
    /// </summary>
    /// <param name="item">The item to which the operation will be applied.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing a string with an error message if the operation fails,  or a
    /// <see cref="Unit"/> value if the operation succeeds.</returns>
    public Either<string, Unit> DetailedApply(T item) => _executable(item).ToEither(_ => Unit.Default, b => !b, Code);
}