using System;
using System.Collections.Generic;
using System.Linq.Expressions;

//Chaow.Expressions enhances the power of expression 

namespace Chaow.Expressions
{
    //ExpressionComparer allows you to test expression for equality
    public static class ExpressionComparer_
    {
        public static void Comparer_Usage()
        {
            //this example shows how to create/use ExpressionComparer

            //you can create comparer by ExpressionComparer.Default
            var comparer = ExpressionComparer.Default;

            //create expressions
            Expression<Func<int, int, int>> add = (x, y) => x + y;
            Expression<Func<int, int, int>> mult = (x, y) => x * y;

            //now compare
            Console.WriteLine(comparer.Equals(add, add));
            Console.WriteLine(comparer.Equals(add, mult));
        }

        public static void Comparer_IEqualityComparer()
        {
            //this example shows you can send ExpressionComparer as IEqualityComparer

            //you can create comparer by ExpressionComparer.Default
            var comparer = ExpressionComparer.Default;

            //create expressions
            Expression<Func<int, int, int>> add = (x, y) => x + y;
            Expression<Func<int, int, int>> mult = (x, y) => x * y;

            //create dictionary
            var dict = new Dictionary<Expression, string>(comparer)
            {
                {add, "add"}, 
                {mult, "multiply"}
            };

            //show result
            Console.WriteLine(dict[mult]);
        }
    }
}