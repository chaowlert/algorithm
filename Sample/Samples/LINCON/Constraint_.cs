using System;
using System.Collections.ObjectModel;
using System.Linq;
using Chaow.Combinatorics;
using Chaow.Extensions;
using Chaow.Numeric;
using Chaow.Samples.Puzzles;

//LINCON is Linq to Constraint, allow you to write constraint programming with Linq

namespace Chaow.LINCON
{
    //This is a collections of LINCON examples
    public static class Constraint_
    {
        public static void Sample_8_Queens()
        {
            //this example shows how to solve 8 queens problem with LINCON
            //go to http://en.wikipedia.org/wiki/Eight_queens_puzzle for more information on puzzle

            //q is list of "ABCDEFGH" which contains 8 items
            var solutions = from q in "ABCDEFGH".ToConstraintList(8)

                                                                   //i is a index from 0 to 6
                                                                   from i in 0.To(6).ToConstraintIndex()

                                                                   //j is a index from 1 to 7
                                                                   from j in (i + 1).To(7).ToConstraintIndex()

                                                                   //all items in q must be different
                                                                   where Constraint.AllDifferent(q)

                                                                   //q[j] cannot put diagonal to q[i]
                                                                   where (q[j] - q[i]).Abs() != j - i

                                                                   //return
                                                                   select q;

            //show result
            Console.WriteLine("First result is {0}", solutions.First().ToString(""));
            Console.WriteLine();
            Console.WriteLine("  A B C D E F G H");
            solutions.First().ForEach((q, i) =>
                Console.WriteLine("{0} " + "_ ".Repeat(q - 'A') + "Q " + "_ ".Repeat(7 - (q - 'A')), i + 1)
                );
            Console.WriteLine();
            Console.WriteLine("Total solutions: {0}", solutions.Count());
        }

        public static void Sample_Einsteins_Puzzle()
        {
            //this example shows how to solve Einstein's puzzle
            //go to http://en.wikipedia.org/wiki/Einstein's_Puzzle for more information on puzzle

            var solutions = from n in ArrayExt.Enum<Nationality>().ToConstraintList(5)
                            from c in ArrayExt.Enum<HouseColor>().ToConstraintList(5)
                            from s in ArrayExt.Enum<Smoke>().ToConstraintList(5)
                            from d in ArrayExt.Enum<Drink>().ToConstraintList(5)
                            from p in ArrayExt.Enum<Pet>().ToConstraintList(5)
                            from i in 0.To(4).ToConstraintIndex()
                            where Constraint.AllDifferent(n)
                            where Constraint.AllDifferent(c)
                            where Constraint.AllDifferent(s)
                            where Constraint.AllDifferent(d)
                            where Constraint.AllDifferent(p)
                            where n[0] == Nationality.Norwegian
                            where d[2] == Drink.Milk
                            where (n[i] == Nationality.British) == (c[i] == HouseColor.Red)
                            where (n[i] == Nationality.German) == (s[i] == Smoke.Prince)
                            where (c[i] == HouseColor.Yellow) == (s[i] == Smoke.DunHill)
                            where (n[i] == Nationality.Danish) == (d[i] == Drink.Tea)
                            where (c[i] == HouseColor.Green) == (d[i] == Drink.Coffee)
                            where (s[i] == Smoke.BlueMaster) == (d[i] == Drink.Beer)
                            where (n[i] == Nationality.Swedish) == (p[i] == Pet.Dog)
                            where (s[i] == Smoke.PallMall) == (p[i] == Pet.Bird)
                            where (c[i] == HouseColor.Green) == (c[i + 1] == HouseColor.White)
                            where (p[i] == Pet.Cat) ? (s[i - 1] == Smoke.Blend || s[i + 1] == Smoke.Blend) : true
                            where ((p[i] == Pet.Horse) ? (s[i - 1] == Smoke.DunHill || s[i + 1] == Smoke.DunHill) : true)
                            where ((n[i] == Nationality.Norwegian) ? (c[i - 1] == HouseColor.Blue || c[i + 1] == HouseColor.Blue) : true)
                            where ((s[i] == Smoke.Blend) ? (d[i - 1] == Drink.Water || d[i + 1] == Drink.Water) : true)
                            select 0.To(4).Select(x =>
                                new
                                {
                                    Nationality = n[x],
                                    HouseColor = c[x],
                                    Smoke = s[x],
                                    Drink = d[x],
                                    Pet = p[x]
                                });

            //show results
            Console.WriteLine("\tNationality\tHouseColor\tSmoke\t\tDrink\t\tPet");
            Console.WriteLine(new string('-', 80));
            solutions.First().ForEach((man, i) =>
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                    i + 1,
                    man.Nationality.ToString().PadRight(12),
                    man.HouseColor.ToString().PadRight(12),
                    man.Smoke.ToString().PadRight(12),
                    man.Drink.ToString().PadRight(12),
                    man.Pet.ToString().PadRight(12))
                );
        }

        public static void Sample_Send_More_Money()
        {
            //this example shows how to solve SEND + MORE = MONEY

            var solutions = from s in 1.To(9).ToConstraintVar()
                            from e in 0.To(9).ToConstraintVar()
                            from n in 0.To(9).ToConstraintVar()
                            from d in 0.To(9).ToConstraintVar()
                            from m in 1.To(9).ToConstraintVar()
                            from o in 0.To(9).ToConstraintVar()
                            from r in 0.To(9).ToConstraintVar()
                            from y in 0.To(9).ToConstraintVar()
                            from c in 0.To(1).ToConstraintList(3)
                            where Constraint.AllDifferent(s, e, n, d, m, o, r, y)
                            where s + m == m * 10 + o - c[2]
                            where e + o == c[2] * 10 + n - c[1]
                            where n + r == c[1] * 10 + e - c[0]
                            where d + e == c[0] * 10 + y
                            select new {s, e, n, d, m, o, r, y};
            Console.WriteLine(solutions.First());
        }

        public static void Sample_Sudoku()
        {
            //this example shows how to solve sudoku with LINCON

            var table = new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 3, 0, 8, 5,
                0, 0, 1, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 5, 0, 7, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 1, 0, 0,
                0, 9, 0, 0, 0, 0, 0, 0, 0,
                5, 0, 0, 0, 0, 0, 0, 7, 3,
                0, 0, 2, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 4, 0, 0, 0, 9
            };

            var solutions = from c in 1.To(9).ToConstraintList(81)
                                                                  from i in 0.To(8).ToConstraintIndex()
                                                                  from j in 0.To(80).ToConstraintIndex()
                                                                  where Constraint.AllDifferent(c[i * 9], c[i * 9 + 1], c[i * 9 + 2],
                                                                      c[i * 9 + 3], c[i * 9 + 4], c[i * 9 + 5],
                                                                      c[i * 9 + 6], c[i * 9 + 7], c[i * 9 + 8])
                                                                  where Constraint.AllDifferent(c[i], c[i + 9], c[i + 18],
                                                                      c[i + 27], c[i + 36], c[i + 45],
                                                                      c[i + 54], c[i + 63], c[i + 72])
                                                                  where Constraint.AllDifferent(c[((i % 3) * 3) + ((i / 3) * 27)],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 1],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 2],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 9],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 10],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 11],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 18],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 19],
                                                                      c[((i % 3) * 3) + ((i / 3) * 27) + 20])
                                                                  where (table[j] > 0) ? c[j] == table[j] : true
                                                                  select c;

            Console.WriteLine(new Sudoku(table));
            Console.WriteLine(new Sudoku(solutions.First().ToArray()));
        }
    }
}