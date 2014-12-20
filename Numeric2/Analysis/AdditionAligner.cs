using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chaow.Expressions;

namespace Chaow.Numeric.Analysis
{
    class AdditionAligner : ExpressionVisitor
    {
        //fields
        List<Expression> _list;

        //public methods
        public Expression Align(Expression exp)
        {
            _list = new List<Expression>();
            Visit(exp);
            return _list.Select(x => new MultiplicationAligner().Align(x))
                        .ToLookup(x => x.Key, x => x.Value, ExpressionComparer.Default)
                        .OrderByDescending(x => x.Key.ToString())
                        .Select(x => x.Aggregate(Expression.Add))
                        .Aggregate(Expression.Add);
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (exp != null && exp.NodeType != ExpressionType.Add)
            {
                _list.Add(exp);
                return exp;
            }
            return base.Visit(exp);
        }
    }

    class MultiplicationAligner : ExpressionVisitor
    {
        //fields
        static readonly ConstantExpression constant = Expression.Constant(1);
        List<Expression> _list;

        //properties
        public Expression Key { get; private set; }
        public Expression Value { get; private set; }

        //public methods
        public MultiplicationAligner Align(Expression exp)
        {
            _list = new List<Expression>();
            Visit(exp);
            var lookup = _list.ToLookup(x =>
            {
                if (x.NodeType == ExpressionType.Constant)
                    return constant;
                if (x.NodeType == ExpressionType.Call)
                {
                    var methodCall = (MethodCallExpression)x;
                    if (methodCall.Method.Name == "Power" || methodCall.Method.Name == "Pow")
                        return methodCall.Object ?? methodCall.Arguments[0];
                }
                return x;
            }, ExpressionComparer.Default);
            Value = lookup.OrderBy(x => x.Key.ToString())
                           .Select(x => x.OrderBy(y => y.ToString())
                                         .Aggregate(Expression.Multiply))
                           .Aggregate(Expression.Multiply);
            if (lookup.Any(x => x.Key != constant))
                Key = lookup.Where(x => x.Key != constant)
                             .OrderBy(x => x.Key.ToString())
                             .Select(x => x.OrderBy(y => y.ToString())
                                           .Aggregate(Expression.Multiply))
                             .Aggregate(Expression.Multiply);
            else
                Key = constant;

            return this;
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (exp != null && exp.NodeType != ExpressionType.Multiply)
            {
                _list.Add(exp);
                return exp;
            }
            return base.Visit(exp);
        }
    }
}