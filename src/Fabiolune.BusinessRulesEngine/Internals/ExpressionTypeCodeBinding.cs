using System.Linq.Expressions;

namespace Fabiolune.BusinessRulesEngine.Internals
{
    internal class ExpressionTypeCodeBinding
    {
        internal BinaryExpression BoolExpression;
        internal string Code;
        internal ParameterExpression TypeExpression;
    }
}