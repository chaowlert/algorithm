using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    public static class ExpressionExt
    {
        public static Expression<TDelegate> Evaluate<TDelegate>(this Expression<TDelegate> exp)
        {
            return new ExpressionEvaluator().Evaluate(exp);
        }

        public static Expression<TDelegate> Evaluate<TDelegate>(this Expression<TDelegate> exp, Func<Expression, bool> exceptionFunc)
        {
            return new ExpressionEvaluator(exceptionFunc).Evaluate(exp);
        }

        public static Expression Evaluate(this Expression exp)
        {
            return new ExpressionEvaluator().Evaluate(exp);
        }

        public static Expression Evaluate(this Expression exp, Func<Expression, bool> exceptionFunc)
        {
            return new ExpressionEvaluator(exceptionFunc).Evaluate(exp);
        }

        public static Expression Replace(this Expression exp, Expression from, Expression to)
        {
            return new ExpressionReplacer(from, to).Replace(exp);
        }

        public static Expression Replace(this Expression exp, Expression from, Expression to, IEqualityComparer<Expression> comparer)
        {
            return new ExpressionReplacer(from, to, comparer).Replace(exp);
        }

        public static bool Contains(this Expression exp, Expression value)
        {
            return new ExpressionFinder(value).Find(exp);
        }

        public static bool Contains(this Expression exp, Expression value, IEqualityComparer<Expression> comparer)
        {
            return new ExpressionFinder(value, comparer).Find(exp);
        }
    }
}