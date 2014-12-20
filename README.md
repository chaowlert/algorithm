# Project Description
This is my collection of algorithms. Migrate from CodePlex

# Features
####1. Extend your .NET language
`Chaow.Extensions` contains several extensions for array, list, date, int, string and more.
You can use the extensions to make you code easier.

```
var matchQuery = new {
    BirthDate = 1.January(2000),    //Create date easily
    Iam = Sex.Man,
    LookingFor = Array.Enum<Sex>(), //Looking for Man, and Woman! (Expand enum)
    AgeRange = 18.To(25)            //Create number range
}
```
You will also be able to write Functional Programming and Dynamic Programming.

```
Func<int, int> fibo = null;
fibo = x => fibo(x - 1) + fibo(x - 2);
fibo = fibo.When(x => x <= 1, x => x); //Able to do pattern matching
fibo = fibo.Memoize();                 //Allow you to do memoization
var result = fibo(38);
```

---
####2. Set of based class libraries
With `Chaow.Numeric`, you will get several new based classes for using in your code.

```
var prime = new Prime();
var isPrime = prime.Contains(142857); //Test for prime number
var primeList = prime.Take(10000);    //Generate prime numbers
```

---
####3. Solve polynomial problems
You can easily analyze your math functions with `Chaow.Numeric2`.

ie. Solve integer of X, Y in {"3XX + 14XY + 6YY - 17X - 23Y - 505 = 0"}
```
var solutions = Diophantine.Parse((x, y) => 3 * x * x + 14 * x * y + 6 * y * y - 17 * x - 23 * y - 505).Solutions;
var x = solutions[0].X0;
var y = solutions[0].Y0;
```
ie2. Find formula from sequence of numbers
```
var formula = MathExt2.PolynomialInterpolation(new Rational[] { 1, 8, 27, 64 });
Console.WriteLine(formula.Rewrite());
//Output is x => x.Power(3)
```

---
####4. Solve nonpolynomial problems
`Chaow.Combinatorics` will allow you to backtrack with any types of problem.

ie. Solve 8 Queens easily, with few lines of code.
```
var solutions = "ABCDEFGH".Backtrack(8);
solutions.BacktrackingModel = BacktrackingModel.Permutation;
solutions.AppendConstraint(set => q =>
    set.All((x, i) => (set.Length - i != (x - q).Abs()))
);
var answer = solutions.First();
```

Finally, may I introduce new language LINCON (Language INtegrated CONstraint programming).
LINCON is strong type constraint programming, which fast, declarative style and fun!

ie3. Solving Einstein's puzzle with LINCON!
```
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
                    new {
                        Nationality = n[x],
                        HouseColor = c[x],
                        Smoke = s[x],
                        Drink = d[x],
                        Pet = p[x]
                    });
var answer = solutions.First();
```

# Modules

| module              | description                                                                                             |
|---------------------|---------------------------------------------------------------------------------------------------------|
| Chaow.Algorithms    | provides data type to solve specific problems ie. binary indexed tree disjoint set, hungarian algorithm |
| Chaow.Combinatorics | enables you to do combinatoric search easier ie. combination, permutation, partition                    |
| Chaow.Expression    | enhances the power of expression ie. exp matcher, exp replacer, exp rewriter                            |
| Chaow.Extensions    | extends .NET framework types ie. int, string, ienumerable, func                                         |
| Chaow.LINCON        | language integrated constraint programming, allows you to write cp with linq                            |
| Chaow.Numeric       | provides various high performance numeric types ie. prime, biginteger, rational                         |
| Chaow.Numeric 2     | allows you to do mathematical analysis ie. diophantine solver, polynomial rewriter                      |

# Links
- [My blog](http://chaowman.bloggang.com) (Thai language)
