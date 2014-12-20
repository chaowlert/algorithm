using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    public sealed class ExpressionComparer : ExpressionCompareVisitor, IEqualityComparer<Expression>
    {
        //public static field
        public static readonly ExpressionComparer Default = new ExpressionComparer();

        //public methods
        public bool Equals(Expression x, Expression y)
        {
            return Visit(x, y);
        }

        public int GetHashCode(Expression obj)
        {
            return obj.NodeType.GetHashCode() ^ obj.Type.GetHashCode();
        }
    }
}