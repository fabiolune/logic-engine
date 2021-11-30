using System.Linq.Expressions;

namespace RulesEngine.Internals
{
    internal class ExpressionTypeCodeBinding
    {
        internal BinaryExpression BoolExpression;
        internal string Code;
        internal ParameterExpression TypeExpression;
    }
}