using System.Linq.Expressions;

namespace LogicEngine.Internals
{
    internal class ExpressionTypeCodeBinding
    {
        internal BinaryExpression BoolExpression;
        internal string Code;
        internal ParameterExpression TypeExpression;
    }
}