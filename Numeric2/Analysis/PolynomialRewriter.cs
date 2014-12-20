using System;
using System.Linq.Expressions;
using Chaow.Expressions;

namespace Chaow.Numeric.Analysis
{
    class PolynomialRewriter
    {
        //fields
        readonly AdditionAligner _aligner = new AdditionAligner();
        readonly ExpressionComparer _comparer = ExpressionComparer.Default;
        readonly ExpressionRewriter _rewriter;

        //constructors
        public PolynomialRewriter()
        {
            _rewriter = new ExpressionRewriter();

            //Identity
            _rewriter.AppendRule<Func<Rational, Rational>>(x => 1 * x, x => x);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => 0 * x, x => 0);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => x + 0, x => x);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => 0 + x, x => x);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => x.Power(1), x => x);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => x.Power(0), x => 1);

            //Commutation
            _rewriter.AppendRule<Func<Rational, Rational, Rational>>(
                p => p["y"].NodeType == ExpressionType.Constant
                     && p["x"].NodeType != ExpressionType.Constant,
                (x, y) => x * y,
                (x, y) => y * x);

            //Association
            _rewriter.AppendRule<Func<Rational, Rational, Rational, Rational>>(
                p => p["x"].NodeType != ExpressionType.Constant
                     || p["y"].NodeType == ExpressionType.Constant,
                (x, y, z) => x * (y * z),
                (x, y, z) => (x * y) * z);
            _rewriter.AppendRule<Func<Rational, Rational, Rational, Rational>>(
                p => p["x"].NodeType == ExpressionType.Constant
                     && p["y"].NodeType != ExpressionType.Constant,
                (x, y, z) => (x * y) * z,
                (x, y, z) => x * (y * z));

            //Distribution
            _rewriter.AppendRule<Func<Rational, Rational, Rational, Rational>>(
                p => p["y"].NodeType != ExpressionType.Constant
                     || p["z"].NodeType != ExpressionType.Constant,
                (x, y, z) => x * (y + z),
                (x, y, z) => x * y + x * z);
            _rewriter.AppendRule<Func<Rational, Rational, Rational, Rational>>(
                p => p["y"].NodeType != ExpressionType.Constant
                     || p["z"].NodeType != ExpressionType.Constant,
                (x, y, z) => (y + z) * x,
                (x, y, z) => y * x + z * x);

            //Factoring
            _rewriter.AppendRule<Func<Rational, Rational, Rational>>((x, y) => x / y, (x, y) => x * y.Power(-1));
            _rewriter.AppendRule<Func<Rational, Rational, Rational>>((x, y) => x - y, (x, y) => x + -1 * y);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => -x, x => -1 * x);
            _rewriter.AppendRule<Func<Rational, Rational, Rational, Rational>>(
                p => p["y"].NodeType == ExpressionType.Constant
                     && p["z"].NodeType == ExpressionType.Constant,
                (x, y, z) => (y * x) + (z * x),
                (x, y, z) => (y + z) * x);
            _rewriter.AppendRule<Func<Rational, Rational, Rational>>(
                p => p["y"].NodeType == ExpressionType.Constant,
                (x, y) => x + y * x,
                (x, y) => (y + 1) * x);
            _rewriter.AppendRule<Func<Rational, Rational, Rational>>(
                p => p["y"].NodeType == ExpressionType.Constant,
                (x, y) => y * x + x,
                (x, y) => (y + 1) * x);
            _rewriter.AppendRule<Func<Rational, Rational>>(x => x + x, x => 2 * x);

            //Exponentation
            _rewriter.AppendRule<Func<Rational, Rational>>(
                p => p["x"].NodeType != ExpressionType.Add,
                x => x * x,
                x => x.Power(2));
            _rewriter.AppendRule<Func<Rational, int, Rational>>(
                p => p["x"].NodeType != ExpressionType.Add,
                (x, y) => x * x.Power(y),
                (x, y) => x.Power(y + 1));
            _rewriter.AppendRule<Func<int, Rational, Rational, Rational>>(
                p => p["x"].NodeType == ExpressionType.Constant && (int)((ConstantExpression)p["x"]).Value > 1,
                (x, y, z) => (y + z).Power(x),
                (x, y, z) => (y + z).Power(x - 1) * (y + z));
            _rewriter.AppendRule<Func<int, Rational, Rational, Rational>>((x, y, z) => (y * z).Power(x), (x, y, z) => y.Power(x) * z.Power(x));
            _rewriter.AppendRule<Func<Rational, int, int, Rational>>((x, y, z) => x.Power(y).Power(z), (x, y, z) => x.Power(y * z));
            _rewriter.AppendRule<Func<Rational, int, int, Rational>>((x, y, z) => x.Power(y) * x.Power(z), (x, y, z) => x.Power(y + z));
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