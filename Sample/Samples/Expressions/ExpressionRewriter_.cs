using System;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    //ExpressionRewriter allows you to rewrite expression from specified rules
    public static class ExpressionRewriter_
    {
        public static void Rewriter_Usage()
        {
            //this example shows how to create/use rewriter

            //create rewriter
            var rewriter = new ExpressionRewriter();

            //use rewriter.AppendRule(from,to) to set rule
            //below is to distribute the multiplication
            rewriter.AppendRule<Func<int, int, int, int>>(
                (a, b, c) => a * (b + c),
                (a, b, c) => (a * b) + (a * c));

            //create expression
            Expression<Func<int, int, int>> exp = (a, b) => a * (b + 1);

            //use rewriter.Rewrite() to rewrite expression
            Console.WriteLine("Before rewrite: {0}", exp.Body);
            Console.WriteLine("After rewrite: {0}", rewriter.Rewrite(exp.Body));
        }

        public static void Rewriter_AppendRule_With_Condition()
        {
            //this example shows how to append rule with condition

            //create rewriter
            var rewriter = new ExpressionRewriter();

            //use rewriter.AppendRule(condition,from,to) to set rule with condition
            //below is to swap is 'a' is parameter and 'b' is constant
            rewriter.AppendRule<Func<int, int, int>>(
                param => param["a"].NodeType == ExpressionType.Parameter && param["b"].NodeType == ExpressionType.Constant,
                (a, b) => a * b,
                (a, b) => b * a);

            //create expression
            Expression<Func<int, int>> exp = x => x * 5;

            //use rewriter.Rewrite() to rewrite expression
            Console.WriteLine("Before rewrite: {0}", exp.Body);
            Console.WriteLine("After rewrite: {0}", rewriter.Rewrite(exp.Body));
        }
    }
}