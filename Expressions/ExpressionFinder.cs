using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    sealed class ExpressionFinder : ExpressionVisitor
    {
        //fields
        readonly IEqualityComparer<Expression> _comparer;
        readonly Expression _value;
        bool _found;

        //constructors
        public ExpressionFinder(Expression value)
        {
            _value = value;
            _comparer = ExpressionComparer.Default;
        }

        public ExpressionFinder(Expression value, IEqualityComparer<Expression> comparer)
        {
            _value = value;
            _comparer = comparer ?? ExpressionComparer.Default;
        }

        //public methods
        public bool Find(Expression exp)
        {
            _found = false;
            Visit(exp);
            return _found;
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (_found)
                return exp;
            if (_comparer.Equals(exp, _value))
            {
                _found = true;
                return exp;
            }
            return base.Visit(exp);
        }
    }
}