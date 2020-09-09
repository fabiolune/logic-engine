using System.Linq.Expressions;

namespace Fabiolune.BusinessRulesEngine.Models
{
    internal class ExpressionTypeCodeBinding
    {
        internal BinaryExpression BoolExpression;
        internal string Code;
        internal ParameterExpression TypeExpression;
    }
}