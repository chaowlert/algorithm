using System;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;
using Chaow.Numeric;

//Chaow.Combinatorics enables you to do combinatoric search easier 

namespace Chaow.Combinatorics
{
    //Backtracker allows you to backtrack over iteration
    public static class Backtracker_
    {
        public static void Backtrack_Create()
        {
            //this example shows how to create backtracker

            //create collection
            IEnumerable<char> alpha = "abc";

            //you can create backtracker by collection.Backtrack(length)
            //length is target length of solution
            var solutions = alpha.Backtrack(2);

            //show results
            //solutions will be any combination of 'a', 'b', 'c' with length of 2
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Backtrack_Permute()
        {
            //this example shows how to create permutaion

            //create backtracker
            var solutions = "abc".Backtrack(2);

            //use backtracker.BacktrackingModel = Permutation to create permutation
            solutions.BacktrackingModel = BacktrackingModel.Permutation;

            //show results
            //with permutation each element is used only once
            //therefore there is no 'aa', 'bb', 'cc'
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Backtrack_Combine()
        {
            //this example shows how to create combination

            //create backtracker
            var solutions = "abc".Backtrack(2);

            //use backtracker.BacktrackingModel = Combination to create combination
            solutions.BacktrackingModel = BacktrackingModel.Combination;

            //show results
            //with combination, 'ab' will equal to 'ba'
            //therefore there is no 'ba', 'ca', 'cb'
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Backtrack_Distinct()
        {
            //this example shows how to create distinct solutions

            //create combination
            //source contains duplicate element 'aa'
            var solutions = "aabc".Backtrack(2);
            solutions.BacktrackingModel = BacktrackingModel.Combination;

            //show results
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
            Console.WriteLine();

            //use backtracker.Distinct = true to generate unique solutions
            solutions.Distinct = true;

            //show results
            //there is only one 'ab' and 'ac'. No duplicated solutions
            Console.WriteLine("below is distinct solution");
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Backtrack_AppendConstraint()
        {
            //this example shows how to set a constraint to the backtracker

            //create backtracker
            var solutions = "abc".Backtrack(2);

            //use backtracker.AppendConstraint(set => item => predicate_body)
            //set is current state of solution
            //item is item to be appended into solution
            //below is to set the constraint that, item must not be 'b'
            solutions.AppendConstraint(set => item => item != 'b');

            //show results
            //solutions will not contain 'b'
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Backtrack_AppendConstraint_2()
        {
            //this example shows how to conditionally set constraint to the backtracker

            //create backtracker
            var solutions = "abc".Backtrack(2);

            //use backtracker.AppendConstraint(set => predicate_body, set => item => predicate_body)
            //insert 2 lambda here
            //set => predicate_body is condition to append the constraint
            //set => item => predicate_body is the constraint
            //set is current state of solution
            //item is item to be appended into solution
            //below is to set the constraint that,
            //item must not be 'b', if it is second item of the solution
            solutions.AppendConstraint(set => set.Length == 1,
                set => item => item != 'b');

            //show results
            //solutions will not contain 'ab', 'bb', 'cb'
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Sample_8_Queens()
        {
            //this example shows how to solve 8 queens problem with backtracker
            //go to http://en.wikipedia.org/wiki/Eight_queens_puzzle for more information on puzzle

            //create 8 items of A to H
            var solutions = "ABCDEFGH".Backtrack(8);

            //each item can be used only once
            //ABCDEFGH is valid
            //AAAAAAAA is not valid
            solutions.BacktrackingModel = BacktrackingModel.Permutation;

            //create a constraint
            //the new queen (q) cannot put diagonal to queens in the set
            solutions.AppendConstraint(set => q =>
                set.All((x, i) => (set.Length - i != (x - q).Abs()))
                );

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

            //select combination of each person
            var men = from n in ArrayExt.Enum<Nationality>()
                      from c in ArrayExt.Enum<HouseColor>()
                      where (n == Nationality.British) == (c == HouseColor.Red)
                      from s in ArrayExt.Enum<Smoke>()
                      where (n == Nationality.German) == (s == Smoke.Prince)
                      where (c == HouseColor.Yellow) == (s == Smoke.DunHill)
                      from d in ArrayExt.Enum<Drink>()
                      where (n == Nationality.Danish) == (d == Drink.Tea)
                      where (c == HouseColor.Green) == (d == Drink.Coffee)
                      where (s == Smoke.BlueMaster) == (d == Drink.Beer)
                      from p in ArrayExt.Enum<Pet>()
                      where (n == Nationality.Swedish) == (p == Pet.Dog)
                      where (s == Smoke.PallMall) == (p == Pet.Bird)
                      select new
                      {
                          Nationality = n,
                          HouseColor = c,
                          Smoke = s,
                          Drink = d,
                          Pet = p
                      };

            //Generate solution
            var solutions = men.Backtrack(5);

            //New guy must not has same properties as persons in sequence
            solutions.AppendConstraint(seq => seq.Any(),
                seq => man => seq.All(x =>
                    x.Nationality != man.Nationality &&
                    x.HouseColor != man.HouseColor &&
                    x.Smoke != man.Smoke &&
                    x.Drink != man.Drink &&
                    x.Pet != man.Pet
                    )
                );

            //Norwegian lives in the first house.
            solutions.AppendConstraint(seq => seq.Length == 0,
                seq => man => man.Nationality == Nationality.Norwegian
                );

            //Guy who drinks milk lives in the third house
            solutions.AppendConstraint(seq => seq.Length <= 2,
                seq => man => (seq.Length == 2) == (man.Drink == Drink.Milk)
                );

            //White house is next to green house
            solutions.AppendConstraint(seq => seq.Any(),
                seq => man => (seq.Last().HouseColor == HouseColor.Green) == (man.HouseColor == HouseColor.White)
                );

            //Guy who pets cat is neighbor with guy who smokes Blend
            solutions.AppendConstraint(seq => seq.Count(x => x.Pet == Pet.Cat || x.Smoke == Smoke.Blend) == 1,
                seq => man => (man.Pet == Pet.Cat) || (man.Smoke == Smoke.Blend)
                );

            //Guy who pets horse is neighbor with guy who smokes DunHill
            solutions.AppendConstraint(seq => seq.Count(x => x.Pet == Pet.Horse || x.Smoke == Smoke.DunHill) == 1,
                seq => man => (man.Pet == Pet.Horse) || (man.Smoke == Smoke.DunHill)
                );

            //Norwegian is neighbor with guy lives in blue house
            solutions.AppendConstraint(seq => seq.Count(x => x.Nationality == Nationality.Norwegian || x.HouseColor == HouseColor.Blue) == 1,
                seq => man => (man.Nationality == Nationality.Norwegian) || (man.HouseColor == HouseColor.Blue)
                );

            //Guy who drinks water is neighbor with guy who smokes Blend
            solutions.AppendConstraint(seq => seq.Count(x => x.Drink == Drink.Water || x.Smoke == Smoke.Blend) == 1,
                seq => man => (man.Drink == Drink.Water) || (man.Smoke == Smoke.Blend)
                );

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

        public static void Advance_Backtrack_Create()
        {
            //this example shows how to create local variable in backtracker

            //use collection.Backtrack(initial_state,
            //                         (state, item, index) => next_state,
            //                         state => yield_predicate)
            //to construct advance backtracker
            //initial_state is first state of a solution
            //(state, item, index) => next_state is function to create next state
            //state => yield_predicate is predicate to determine this state is completed
            //below is equal
            var solutions = "ABC".Backtrack(2);
            var solutions2 = "ABC".Backtrack(new char[0], //initial state is empty array of char
                (set, item, i) => set.Append(item), //next state is to append new item to array
                set => set.Length == 2); //completed state is when lenght is 2

            //shows result
            solutions.Zip(solutions2).ForEach(zipSol => Console.WriteLine("{0}\t{1}", zipSol.Item1.ToString(""), zipSol.Item2.ToString("")));
        }

        public static void Advance_Backtrack_Break()
        {
            //this example shows how to set break to backtracker

            //create backtracker
            var solutions = "ABC".Backtrack(string.Empty, //start with empty string
                (str, item, i) => str + item, //append new char to string
                str => str.Length == 2); //complete when string length is 2

            //use backtracker.Break(state => break_predicate) to set break
            //below is to break when solution is 'AC', 'BA', 'CB'
            solutions.AppendBreak(str => str.In("AC", "BA", "CB"));

            //show results
            solutions.ForEach(sol => Console.WriteLine(sol.ToString("")));
        }

        public static void Advance_Backtrack_ContinueOnYield()
        {
            //this example shows how to keep finding new solution even state is completed

            //create backtracker
            var solutions = 2.To(5).Backtrack(1, //start with 1
                (x, item, i) => x * item, //multiply number with new item
                x => true); //always complete

            //2 * 3 equal to 3 * 2, therefore skip 3 * 2
            solutions.BacktrackingModel = BacktrackingModel.Combination;

            //use backtracker.ContinueOnYield = true to keep keep finding new solution even state is completed
            //2 * 3 is completed state, but backtracker will keep finding next item to solution
            //next item is 4, therefore 2 * 3 * 4 will be next solution
            solutions.ContinueOnYielded = true;

            //show results
            solutions.ForEach(sol => Console.WriteLine(sol.ToString(" * ")));
        }

        public static void Advance_Backtrack_SelectResults()
        {
            //this example shows how to select the result of completed state

            //create backtracker
            var solutions = 2.To(5).Backtrack(1, //start with 1
                (x, item, i) => x * item, //multiply number with new item
                x => true); //always complete
            solutions.BacktrackingModel = BacktrackingModel.Combination;
            solutions.ContinueOnYielded = true;

            //use backtracker.SelectResults() to select the result of completed state
            //therefore the solution will not contain {2, 3},  but it will contain 6
            solutions.SelectResults().ForEach(Console.WriteLine);
        }

        public static void Sample_River_Crossing()
        {
            //this example shows how to solve river crossing puzzle
            //go to http://en.wikipedia.org/wiki/River_IQ_Test for more information on puzzle
            //go to http://freeweb.siol.net/danej/riverIQGame.swf for the game

            //create some variables
            var allPeople = Cross.Thief | Cross.Police | Cross.Father | Cross.Mother | Cross.Son | Cross.Son2 | Cross.Daughter | Cross.Daughter2;

            //STEP 1 - create combination of crossing
            var crossing = ArrayExt.Enum<Cross>().Backtrack(
                //Seed - number of people, and people
                new {Length = 0, Value = (Cross)0},

                //Func - add number of people, add people
                (current, item, i) => new {Length = current.Length + 1, Value = current.Value | item},

                //Yield - must contains police, father, or mother
                current => (current.Value & (Cross.Police | Cross.Father | Cross.Mother)) > 0
                );

            //thief-police is the same as police-thief, therefore skip police-thief
            crossing.BacktrackingModel = BacktrackingModel.Combination;

            //Answer can be 1 or 2 people on boat
            crossing.ContinueOnYielded = true;

            //Stop backtracking when there are more than 2 people
            crossing.AppendBreak(current => current.Length > 2);

            //Thief must be with police
            crossing.AppendConstraint(current => current.Value == Cross.Thief,
                current => item => item == Cross.Police);

            //Father cannot be with daughter
            crossing.AppendConstraint(current => current.Value == Cross.Father,
                current => item => item != Cross.Daughter && item != Cross.Daughter2);

            //Mother cannot be with son
            crossing.AppendConstraint(current => current.Value == Cross.Mother,
                current => item => item != Cross.Son && item != Cross.Son2);

            //shows result
            Console.WriteLine("below is combination of crossing");
            crossing.SelectResults(c => c.Value).ForEach(c => Console.WriteLine(c));
            Console.WriteLine();

            //----------------------------------------------------------------------
            //STEP 2 - create solutions based on combination of crossing
            var solutions = crossing.SelectResults(c => c.Value).Backtrack(
                //Seed - list of people on landA and landB
                new
                {
                    LandA = allPeople,
                    LandB = (Cross)0,
                    BoatOnLandA = true,
                    History = new[] {new {LandA = allPeople, BoatOnLandA = true}}
                },

                //Func - add/subtract crossing people to/from LandA, LandB
                (last, item, i) => new
                {
                    LandA = last.LandA ^ item,
                    LandB = last.LandB ^ item,
                    BoatOnLandA = !last.BoatOnLandA,
                    History = last.History.Append(new {LandA = last.LandA ^ item, BoatOnLandA = !last.BoatOnLandA})
                },

                //Yield - all people on LandB
                last => last.LandB == allPeople
                );

            //Thief cannot be with others without police
            solutions.AppendBreak(last =>
                ((last.LandA & (Cross.Thief | Cross.Police)) == Cross.Thief) && //no police
                (last.LandA != Cross.Thief) //has others
                );
            solutions.AppendBreak(last =>
                ((last.LandB & (Cross.Thief | Cross.Police)) == Cross.Thief) && //no police
                (last.LandB != Cross.Thief) //has others
                );

            //Son cannot be with mother without father
            solutions.AppendBreak(last =>
                ((last.LandA & (Cross.Father | Cross.Mother)) == Cross.Mother) && //no father
                ((last.LandA & (Cross.Son | Cross.Son2)) > 0) //has sons
                );
            solutions.AppendBreak(last =>
                ((last.LandB & (Cross.Father | Cross.Mother)) == Cross.Mother) && //no father
                ((last.LandB & (Cross.Son | Cross.Son2)) > 0) //has sons
                );

            //Daughter cannot be with father without mother
            solutions.AppendBreak(last =>
                ((last.LandA & (Cross.Father | Cross.Mother)) == Cross.Father) && //no mother
                ((last.LandA & (Cross.Daughter | Cross.Daughter2)) > 0) //has daugthers
                );
            solutions.AppendBreak(last =>
                ((last.LandB & (Cross.Father | Cross.Mother)) == Cross.Father) && //no mother
                ((last.LandB & (Cross.Daughter | Cross.Daughter2)) > 0) //has daugthers
                );

            //Must have people to cross
            solutions.AppendConstraint(last => last.BoatOnLandA,
                last => item => (last.LandA & item) == item);
            solutions.AppendConstraint(last => !last.BoatOnLandA,
                last => item => (last.LandB & item) == item);

            //Check history
            solutions.AppendConstraint(last => item => !last.History.Contains(new {LandA = last.LandA ^ item, BoatOnLandA = !last.BoatOnLandA}));

            //show result
            Console.WriteLine("Now start crossing");
            solutions.SelectResults().First().History.ForEach(x =>
            {
                Console.WriteLine(x.LandA);
                Console.WriteLine(allPeople ^ x.LandA);
                Console.WriteLine("=============================================================");
            });
        }
    }

    public enum Nationality
    {
        British,
        Swedish,
        Danish,
        Norwegian,
        German,
    }

    public enum HouseColor
    {
        Red,
        Green,
        White,
        Yellow,
        Blue,
    }

    public enum Smoke
    {
        PallMall,
        DunHill,
        Blend,
        BlueMaster,
        Prince,
    }

    public enum Drink
    {
        Tea,
        Coffee,
        Milk,
        Beer,
        Water,
    }

    public enum Pet
    {
        Dog,
        Bird,
        Cat,
        Horse,
        Fish,
    }

    [Flags]
    public enum Cross
    {
        Thief = 1,
        Police = 2,
        Father = 4,
        Mother = 8,
        Son = 16,
        Son2 = 32,
        Daughter = 64,
        Daughter2 = 128,
    }
}