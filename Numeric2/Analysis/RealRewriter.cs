using System;
using System.Linq.Expressions;
using Chaow.Expressions;

namespace Chaow.Numeric.Analysis
{
    class RealRewriter
    {
        //fields
        readonly AdditionAligner _aligner = new AdditionAligner();
        readonly ExpressionComparer _comparer = ExpressionComparer.Default;
        readonly ExpressionRewriter _rewriter;

        //constructors
        public RealRewriter()
        {
            _rewriter = new ExpressionRewriter(exp => exp.NodeType == ExpressionType.Call && ((MethodCallExpression)exp).Method.Name == "Diff");

            //Identity
            _rewriter.AppendRule<Func<double, double>>(x => 1.0 * x, x => x);
            _rewriter.AppendRule<Func<double, double>>(x => 0.0 * x, x => 0.0);
            _rewriter.AppendRule<Func<double, double>>(x => x + 0.0, x => x);
            _rewriter.AppendRule<Func<double, double>>(x => 0.0 + x, x => x);
            _rewriter.AppendRule<Func<double, double>>(x => Math.Pow(x, 1.0), x => x);
            _rewriter.AppendRule<Func<double, double>>(x => Math.Pow(1.0, x), x => 1.0);
            _rewriter.AppendRule<Func<double, double>>(x => Math.Pow(x, 0.0), x => 1.0);
            _rewriter.AppendRule<Func<double, double>>(x => Math.Pow(0.0, x), x => 0.0);

            //Commutation
            _rewriter.AppendRule<Func<double, double, double>>(
                p => p["y"].NodeType == ExpressionType.Constant
                     && p["x"].NodeType != ExpressionType.Constant,
                (x, y) => x * y,
                (x, y) => y * x);

            //Association
            _rewriter.AppendRule<Func<double, double, double, double>>(
                p => p["x"].NodeType != ExpressionType.Constant
                     || p["y"].NodeType == ExpressionType.Constant,
                (x, y, z) => x * (y * z),
                (x, y, z) => (x * y) * z);
            _rewriter.AppendRule<Func<double, double, double, double>>(
                p => p["x"].NodeType == ExpressionType.Constant
                     && p["y"].NodeType != ExpressionType.Constant,
                (x, y, z) => (x * y) * z,
                (x, y, z) => x * (y * z));

            //Distribution
            _rewriter.AppendRule<Func<double, double, double, double>>(
                p => p["y"].NodeType != ExpressionType.Constant
                     || p["z"].NodeType != ExpressionType.Constant,
                (x, y, z) => x * (y + z),
                (x, y, z) => x * y + x * z);
            _rewriter.AppendRule<Func<double, double, double, double>>(
                p => p["y"].NodeType != ExpressionType.Constant
                     || p["z"].NodeType != ExpressionType.Constant,
                (x, y, z) => (y + z) * x,
                (x, y, z) => y * x + z * x);

            //Factoring
            _rewriter.AppendRule<Func<double, double, double>>((x, y) => x / y, (x, y) => x * Math.Pow(y, -1.0));
            _rewriter.AppendRule<Func<double, double, double>>((x, y) => x - y, (x, y) => x + -1.0 * y);
            _rewriter.AppendRule<Func<double, double>>(x => -x, x => -1.0 * x);
            _rewriter.AppendRule<Func<double, double, double, double>>(
                p => p["y"].NodeType == ExpressionType.Constant
                     && p["z"].NodeType == ExpressionType.Constant,
                (x, y, z) => (y * x) + (z * x),
                (x, y, z) => (y + z) * x);
            _rewriter.AppendRule<Func<double, double, double>>(
                p => p["y"].NodeType == ExpressionType.Constant,
                (x, y) => x + y * x,
                (x, y) => (y + 1.0) * x);
            _rewriter.AppendRule<Func<double, double, double>>(
                p => p["y"].NodeType == ExpressionType.Constant,
                (x, y) => y * x + x,
                (x, y) => (y + 1.0) * x);
            _rewriter.AppendRule<Func<double, double>>(x => x + x, x => 2.0 * x);

            //Exponentation
            _rewriter.AppendRule<Func<double, double>>(
                p => p["x"].NodeType != ExpressionType.Add,
                x => x * x,
                x => Math.Pow(x, 2.0));
            _rewriter.AppendRule<Func<double, int, double>>(
                p => p["x"].NodeType != ExpressionType.Add,
                (x, y) => x * Math.Pow(x, y),
                (x, y) => Math.Pow(x, y + 1.0));
            _rewriter.AppendRule<Func<int, double, double, double>>(
                p => p["x"].NodeType == ExpressionType.Constant && Math.Abs((double)((ConstantExpression)p["x"]).Value) > 1.0,
                (x, y, z) => Math.Pow(y + z, x),
                (x, y, z) => Math.Pow(y + z, x - 1.0) * (y + z));
            _rewriter.AppendRule<Func<double, double, double, double>>((x, y, z) => Math.Pow(y * z, x), (x, y, z) => Math.Pow(y, x) * Math.Pow(z, x));
            _rewriter.AppendRule<Func<double, double, double, double>>((x, y, z) => Math.Pow(Math.Pow(x, y), z), (x, y, z) => Math.Pow(x, y * z));
            _rewriter.AppendRule<Func<double, double, double, double>>((x, y, z) => Math.Pow(x, y) * Math.Pow(x, z), (x, y, z) => Math.Pow(x, y + z));
            _rewriter.AppendRule<Func<double, double>>(x => Math.Sqrt(x), x => Math.Pow(x, 0.5));
            _rewriter.AppendRule<Func<double, double>>(x => Math.Log10(x), x => Math.Log(x, 10.0));
            _rewriter.AppendRule<Func<double, double>>(x => Math.Abs(x) * Math.Abs(x), x => Math.Pow(x, 2.0));

            //general differentation
            _rewriter.AppendRule<Func<double, double, double>>(
                (x, y) => MathExt2.Diff(x * y),
                (x, y) => MathExt2.Diff(x) * y + x * MathExt2.Diff(y));
            _rewriter.AppendRule<Func<double, double, double>>(
                (x, y) => MathExt2.Diff(x + y),
                (x, y) => MathExt2.Diff(x) + MathExt2.Diff(y));

            //simple differentiation
            _rewriter.AppendRule<Func<double, double>>(
                p => p["c"].NodeType == ExpressionType.Constant,
                c => MathExt2.Diff(c),
                c => 0.0);
            _rewriter.AppendRule<Func<double, double>>(
                p => p["x"].NodeType == ExpressionType.Parameter,
                x => MathExt2.Diff(x),
                x => 1.0);
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Abs(x)),
                x => MathExt2.Diff(x) * x * Math.Pow(Math.Abs(x), -1.0));
            _rewriter.AppendRule<Func<double, double, double>>(
                p => p["c"].NodeType == ExpressionType.Constant,
                (x, c) => MathExt2.Diff(Math.Pow(x, c)),
                (x, c) => MathExt2.Diff(x) * c * Math.Pow(x, c - 1.0));

            //exponential and logarithm differentiation
            _rewriter.AppendRule<Func<double, double, double>>(
                p => p["c"].NodeType == ExpressionType.Constant,
                (c, x) => MathExt2.Diff(Math.Pow(c, x)),
                (c, x) => MathExt2.Diff(x) * Math.Pow(c, x) * Math.Log(c));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Exp(x)),
                x => Math.Exp(x));
            _rewriter.AppendRule<Func<double, double, double>>(
                p => p["c"].NodeType == ExpressionType.Constant,
                (c, x) => MathExt2.Diff(Math.Log(x, c)),
                (c, x) => MathExt2.Diff(x) * Math.Pow(x * Math.Log(c), -1.0));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Log(x)),
                x => MathExt2.Diff(x) * Math.Pow(x, -1.0));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Pow(x, x)),
                x => MathExt2.Diff(x) * Math.Pow(x, x) * (1.0 + Math.Log(x)));

            //trigonometric differentiation
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Sin(x)),
                x => MathExt2.Diff(x) * Math.Cos(x));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Cos(x)),
                x => MathExt2.Diff(x) * -1.0 * Math.Sin(x));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Tan(x)),
                x => MathExt2.Diff(x) * Math.Pow(Math.Cos(x), -2.0));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Asin(x)),
                x => MathExt2.Diff(x) * Math.Pow(1.0 - Math.Pow(x, 2.0), -0.5));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Acos(x)),
                x => MathExt2.Diff(x) * -1.0 * Math.Pow(1.0 - Math.Pow(x, 2.0), -0.5));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Atan(x)),
                x => MathExt2.Diff(x) * Math.Pow(1.0 + Math.Pow(x, 2.0), -1.0));

            //hyperbolic differentiation
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Sinh(x)),
                x => MathExt2.Diff(x) * Math.Cosh(x));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Cosh(x)),
                x => MathExt2.Diff(x) * Math.Sinh(x));
            _rewriter.AppendRule<Func<double, double>>(
                x => MathExt2.Diff(Math.Tan(x)),
                x => MathExt2.Diff(x) * Math.Pow(Math.Cosh(x), -2.0));
        }

        //public methods
        public Expression<TDelegate> Rewrite<TDelegate>(Expression<TDelegate> exp)
        {
            var current = _rewriter.Rewrite(exp.Body);
            Expression previous = null;

            while (!_comparer.Equals(current, previous))
            {
                previous = current;
                current = _aligner.Align(current);
                current = _rewriter.Rewrite(current);
            }

            return Expression.Lambda<TDelegate>(current, exp.Parameters);
        }
    }
}