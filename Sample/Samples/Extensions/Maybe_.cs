using System;
using System.Collections.Generic;

namespace Chaow.Extensions
{
    //Maybe allows you to write Maybe monad
    public static class Maybe_
    {
        public static void TryWith()
        {
            //TryParse with TryWith
            var i = "123".TryWith(int.TryParse, 0);
            Console.WriteLine(i);

            //TryGetValue with TryWith
            var dict = new Dictionary<string, int> 
            { 
                { "a", 1 },
            };
            var val = "a".TryWith(dict.TryGetValue, 0);
            Console.WriteLine(val);
        }

        public static void MaybeMonad()
        {
            var p1 = "a";
            var p2 = "bbb";

            //Validation
            var a = (from id in p1.AsMaybe()
                     where id.Length > 1
                     select id).GetValueOrDefault();
            Console.WriteLine("Get p1 if length > 1: {0}", a == null ? "{null}" : a.ToString());

            var b = (from id in p2.AsMaybe()
                     where id.Length > 1
                     select id).GetValueOrDefault();
            Console.WriteLine("Get p2 if length > 1: {0}", b == null ? "{null}" : b.ToString());

            //Association
            var c = (from id1 in p1.AsMaybe()
                     from id2 in p2.AsMaybe()
                     let x = id1 + id2
                     select x.Length).AsNullable();
            Console.WriteLine("Get length of p1+p2: {0}", c == null ? "{null}" : c.ToString());

            var p3 = default(int?);
            var d = (from id1 in p1.AsMaybe()
                     from id3 in p3.AsMaybe()
                     let x = id1 + id3
                     select x.Length).AsNullable();
            Console.WriteLine("Get length of p1+p3: {0}", d == null ? "{null}" : d.ToString());

            //chaining
            var p4 = Tuple.Create(Tuple.Create(Tuple.Create(1)));
            var e = (from id in p4.AsMaybe()
                     select id.Item1 into id
                     select id.Item1 into id
                     select id.Item1).AsNullable();
            Console.WriteLine("Get p4.Item1.Item1.Item1: {0}", e == null ? "{null}" : e.ToString());

            var p5 = Tuple.Create(default(Tuple<Tuple<int>>));
            var f = (from id in p5.AsMaybe()
                     select id.Item1 into id
                     select id.Item1 into id
                     select id.Item1).AsNullable();
            Console.WriteLine("Get p5.Item1.Item1.Item1: {0}", f == null ? "{null}" : f.ToString());

            //throw
            Console.WriteLine("Get p5.Item1.Item1.Item1, if no value throw error");
            var g = (from id in p5.AsMaybe()
                     select id.Item1 into id
                     select id.Item1 into id
                     select id.Item1).GetValueOrThrow<InvalidOperationException>();
            Console.WriteLine(g == null ? "{null}" : g.ToString());
        }
    }
}
