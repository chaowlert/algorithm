using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    //BinaryBracketing allows you to do binary operation over collection
    public static class BinaryBracketing_
    {
        public static void BinaryBracketing_Create()
        {
            //this example shows how to create binary bracketing

            //create binary operation
            Func<string, string, string> bracket = (a, b) => string.Format("({0} {1})", a, b);

            //use collection.BinaryBracketing(operation) to create binary bracketing
            var solutions = new[] {"a", "b", "c", "d"}.BinaryBracketing(bracket);

            //show results
            solutions.ForEach(s => Console.WriteLine(s));
        }

        public static void BinaryBracketing_Create_2()
        {
            //this example shows how to apply many operation to binary bracketing

            //create binary operations
            Func<string, string, string> add = (a, b) => string.Format("({0}+{1})", a, b);
            Func<string, string, string> sub = (a, b) => string.Format("({0}-{1})", a, b);
            Func<string, string, string> mult = (a, b) => string.Format("({0}*{1})", a, b);

            //use collection.BinaryBracketing(operations) to create binary bracketing
            var solutions = new[] {"a", "b", "c", "d"}.BinaryBracketing(new[] {add, sub, mult});

            //show results
            solutions.ForEach(s => Console.WriteLine(s));
        }

        public static void BinaryBracketing_Count()
        {
            //this example shows how to count binary bracketing

            //create binary operations
            Func<string, string, string> add = (a, b) => string.Format("({0}+{1})", a, b);
            Func<string, string, string> sub = (a, b) => string.Format("({0}-{1})", a, b);
            Func<string, string, string> mult = (a, b) => string.Format("({0}*{1})", a, b);

            //create binary bracketing
            var solutions = new[] {"a", "b", "c", "d"}.BinaryBracketing(new[] {add, sub, mult});

            //use binaryBracketing.Count to do counting
            //this class has algorithm to do fast counting
            Console.WriteLine(solutions.Count);
        }

        public static void BinaryBracketing_Index()
        {
            //this example shows how to get result from an index

            //create binary operations
            Func<string, string, string> add = (a, b) => string.Format("({0}+{1})", a, b);
            Func<string, string, string> sub = (a, b) => string.Format("({0}-{1})", a, b);
            Func<string, string, string> mult = (a, b) => string.Format("({0}*{1})", a, b);

            //create binary bracketing
            var solutions = new[] {"a", "b", "c", "d"}.BinaryBracketing(new[] {add, sub, mult});

            //use binaryBracketing[index] to get item from index
            //this class has algorithm to do fast getting item from index
            Console.WriteLine(solutions[3]);
        }

        public static void Sample_Algebra()
        {
            //this example shows how to compute 1,2,5,8 to value 1-100

            //create numbers
            var nums = new Expression[]
            {
                Expression.Constant(1.0),
                Expression.Constant(2.0),
                Expression.Constant(5.0),
                Expression.Constant(8.0)
            };

            //create operations
            var operations = new Func<Expression, Expression, Expression>[]
            {
                Expression.Add,
                Expression.Subtract,
                Expression.Multiply,
                Expression.Divide
            };

            //start compute
            var solutions = from numsPermute in nums.Permute()
                                            from opsPermute in operations.Permute(3, CombinatoricModel.Repetition)
                                            from results in numsPermute.BinaryBracketing(opsPermute)
                                            let computed = Expression.Lambda<Func<double>>(results).Compile()()
                                            where computed > 0.0 &&
                                                  computed <= 100.0 &&
                                                  Math.Floor(computed) == computed
                                            group results by computed
                                            into g
                                            orderby g.Key
                                            select string.Format("{0} = {1}", g.Key, g.First());

            solutions.ForEach(s => Console.WriteLine(s));
        }
    }
}