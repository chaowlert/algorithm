using System;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    //ExpressionExt extends functionality for expression
    public static class ExpressionExt_
    {
        public static void Evaluate()
        {
            //this example shows how to evaluate an expression

            //create expression of 1 + 1
            var exp = Expression.Add(Expression.Constant(1), Expression.Constant(1));

            //use expression.Evaluate() to evaluate the expression
            Console.WriteLine(exp.Evaluate());
        }

        public static void Replace()
        {
            //this example shows how to replace an expression

            //create expression of 1 + 1
            var exp = Expression.Add(Expression.Constant(1), Expression.Constant(1));

            //use expression.Replace(from, to) to replace expression
            //below replaces 1 with 2
            Console.WriteLine(exp.Replace(Expression.Constant(1), Expression.Constant(2)));
        }

        public static void Contains()
        {
            //this example shows how to check whether specified item is in the expression

            //create expression of 1 + 1
            var exp = Expression.Add(Expression.Constant(1), Expression.Constant(1));

            //use expression.Contains(item)
            //below is to check whether expression contains 1
            Console.WriteLine(exp.Contains(Expression.Constant(1)));
        }
    }
}