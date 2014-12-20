using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    sealed class Nominator : ExpressionVisitor
    {
        //fields
        readonly HashSet<Expression> _candidates = new HashSet<Expression>();
        readonly HashSet<ParameterExpression> _exception = new HashSet<ParameterExpression>();
        readonly Func<Expression, bool> _exceptionFunc;
        bool _pass;

        //constructors
        public Nominator()
        {
            _exceptionFunc = null;
        }

        public Nominator(Func<Expression, bool> exceptionFunc)
        {
            _exceptionFunc = exceptionFunc;
        }

        //public methods
        public HashSet<Expression> Nominate(Expression exp)
        {
            _candidates.Clear();
            _exception.Clear();
            Visit(exp);
            return _candidates;
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;
            var oldHasParam = _pass;
            _pass = false;
            base.Visit(exp);
            if (!_pass)
            {
                if (exp.NodeType == ExpressionType.Parameter && !_exception.Contains((ParameterExpression)exp))
                    _pass = true;
                else if (_exceptionFunc != null && _exceptionFunc(exp))
                    _pass = true;
                else if (exp.NodeType != ExpressionType.Constant && _exception.Count == 0)
                    _candidates.Add(exp);
            }
            _pass |= oldHasParam;
            return exp;
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            foreach (var p in lambda.Parameters)
                _exception.Add(p);

            var exp = base.VisitLambda(lambda);

            foreach (var p in lambda.Parameters)
                _exception.Remove(p);

            return exp;
        }
    }
}