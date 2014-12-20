using System;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    //BaseCombinatoric is based class for combinatoric classes
    public static class BaseCombinatoric_
    {
        public static void BaseCombinatoric_As_Enumerable()
        {
            //this example shows you can enumerate on Combinatoric

            //create combinatoric
            var permute = "ABC".Permute();

            //show results
            permute.ForEach(p => Console.WriteLine(p.ToString("")));
        }

        public static void BaseCombinatoric_As_Collection()
        {
            //this example shows you can use ICollection's methods on Combinatoric

            //create combinatoric
            var permute = "ABC".Permute();

            //you can use combinatoric.Count and combinatoric.Contains
            Console.WriteLine("Permuation of ABC has {0} solutions", permute.Count);
            Console.WriteLine("Does Permuatation of ABC contains BCA?");
            Console.WriteLine(permute.Contains("BCA"));
        }

        public static void BaseCombinatoric_As_List()
        {
            //this example shows you can use IList's methods on Combinatoric

            //create combinatoric
            var permute = "ABC".Permute();

            //you can use combinatoric[index] and combinatoric.IndexOf(item)
            Console.WriteLine("3rd Permutation of ABC is {0}", permute[3].ToString(""));
            Console.WriteLine("Index of BCA in Permutation of ABC is {0}", permute.IndexOf("BCA"));
        }
    }
}