using System.Linq.Expressions;

namespace LogicEngine.Internals;

internal struct ExpressionTypeCodeBinding
{
    public ExpressionTypeCodeBinding(Expression testExpression, ParameterExpression typeExpression, string code)
    {
        TestExpression = testExpression;
        TypeExpression = typeExpression;
        Code = code;
    }

    internal Expression TestExpression { get; }
    internal string Code { get; init; }
    internal ParameterExpression TypeExpression { get; }
}