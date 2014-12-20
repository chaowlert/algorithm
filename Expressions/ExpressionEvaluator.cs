using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    sealed class ExpressionEvaluator : ExpressionVisitor
    {
        //fields
        readonly Nominator _nominator;
        HashSet<Expression> _candidates;

        //constructors
        public ExpressionEvaluator()
        {
            _nominator = new Nominator();
        }

        public ExpressionEvaluator(Func<Expression, bool> exceptionFunc)
        {
            _nominator = new Nominator(exceptionFunc);
        }

        //public methods
        public Expression<TDelegate> Evaluate<TDelegate>(Expression<TDelegate> exp)
        {
            return Expression.Lambda<TDelegate>(Evaluate(exp.Body), exp.Parameters);
        }

        public Expression Evaluate(Expression exp)
        {
            _candidates = _nominator.Nominate(exp);
            if (_candidates.Count == 0)
                return exp;
            return Visit(exp);
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;
            if (_candidates.Contains(exp))
                return evaluate(exp);
            return base.Visit(exp);
        }

        //static methods
        static Expression evaluate(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
                return exp;

            var lambda = Expression.Lambda(exp);
            var fn = lambda.Compile();
            return Expression.Constant(fn.DynamicInvoke(null), exp.Type);
        }
    }
}