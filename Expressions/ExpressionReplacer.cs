using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    sealed class ExpressionReplacer : ExpressionVisitor
    {
        //fields
        readonly IEqualityComparer<Expression> _comparer;
        readonly Expression _from;
        readonly Expression _to;

        //constructors
        public ExpressionReplacer(Expression from, Expression to)
        {
            _from = from;
            _to = to;
            _comparer = ExpressionComparer.Default;
        }

        public ExpressionReplacer(Expression from, Expression to, IEqualityComparer<Expression> comparer)
        {
            _from = from;
            _to = to;
            _comparer = comparer ?? ExpressionComparer.Default;
        }

        //public methods
        public Expression Replace(Expression exp)
        {
            return Visit(exp);
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (_comparer.Equals(exp, _from))
                return _to;
            return base.Visit(exp);
        }
    }
}