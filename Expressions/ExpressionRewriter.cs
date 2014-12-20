using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Chaow.Expressions
{
    public sealed class ExpressionRewriter : ExpressionVisitor
    {
        //fields
        readonly ExpressionRuleApplier _applier = new ExpressionRuleApplier();
        readonly ExpressionEvaluator _evaluator;
        readonly List<ExpressionRule> _rules = new List<ExpressionRule>();

        //constructors
        public ExpressionRewriter()
        {
            _evaluator = new ExpressionEvaluator();
        }

        public ExpressionRewriter(Func<Expression, bool> exceptionFunc)
        {
            _evaluator = new ExpressionEvaluator(exceptionFunc);
        }

        //public methods
        public ExpressionRewriter AppendRule<TDelegate>(Expression<TDelegate> from, Expression<TDelegate> to)
        {
            return AppendRule(null, from, to);
        }

        public ExpressionRewriter AppendRule<TDelegate>(Func<ReadOnlyDictionary<string, Expression>, bool> predicate, Expression<TDelegate> from, Expression<TDelegate> to)
        {
            _rules.Add(new ExpressionRule(predicate, _evaluator.Evaluate(from), _evaluator.Evaluate(to)));
            return this;
        }

        public Expression Rewrite(Expression exp)
        {
            Expression previous = null;
            exp = _evaluator.Evaluate(exp);

            while (exp != previous)
            {
                previous = exp;
                exp = Visit(exp);
            }
            return exp;
        }

        //protected methods
        public override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            exp = base.Visit(exp);

            var previous = exp;
            foreach (var rule in _rules)
                exp = _applier.Apply(exp, rule);
            if (previous != exp)
                exp = _evaluator.Evaluate(exp);
            return exp;
        }

        //child classes

        sealed class ExpressionMatcher : ExpressionCompareVisitor
        {
            //fields
            readonly ExpressionComparer _comparer = ExpressionComparer.Default;
            readonly Dictionary<string, Expression> _dict = new Dictionary<string, Expression>();

            //public methods
            public bool Match(Expression exp, LambdaExpression pattern, out ReadOnlyDictionary<string, Expression> dict)
            {
                _dict.Clear();
                foreach (var p in pattern.Parameters)
                    _dict.Add(p.Name, null);
                var success = Visit(exp, pattern.Body);
                dict = new ReadOnlyDictionary<string, Expression>(_dict);
                return success;
            }

            //protected methods
            protected override bool Visit(Expression exp, Expression exp2)
            {
                if (exp != null && exp2 != null && exp2.NodeType == ExpressionType.Parameter)
                {
                    var p2 = (ParameterExpression)exp2;
                    Expression value;
                    if (_dict.TryGetValue(p2.Name, out value))
                    {
                        if (value == null)
                        {
                            if (p2.Type.IsAssignableFrom(exp.Type))
                            {
                                _dict[p2.Name] = exp;
                                return true;
                            }
                            return false;
                        }
                        return _comparer.Equals(exp, value);
                    }
                }
                return base.Visit(exp, exp2);
            }
        }

        struct ExpressionRule
        {
            //fields
            readonly LambdaExpression _from;
            readonly Func<ReadOnlyDictionary<string, Expression>, bool> _predicate;
            readonly LambdaExpression _to;

            public ExpressionRule(Func<ReadOnlyDictionary<string, Expression>, bool> predicate, LambdaExpression from, LambdaExpression to)
            {
                _predicate = predicate;
                _from = from;
                _to = to;
            }

            //properties
            public Func<ReadOnlyDictionary<string, Expression>, bool> Predicate
            {
                get { return _predicate; }
            }

            public LambdaExpression From
            {
                get { return _from; }
            }

            public LambdaExpression To
            {
                get { return _to; }
            }

            //constructors
        }

        sealed class ExpressionRuleApplier : ExpressionVisitor
        {
            //fields
            readonly ExpressionMatcher _matcher = new ExpressionMatcher();
            ReadOnlyDictionary<string, Expression> _dict;

            //public methods
            public Expression Apply(Expression exp, ExpressionRule rule)
            {
                if (_matcher.Match(exp, rule.From, out _dict))
                {
                    if (rule.Predicate == null || rule.Predicate(_dict))
                        return Visit(rule.To.Body);
                }
                return exp;
            }

            //protected methods
            protected override Expression VisitParameter(ParameterExpression p)
            {
                Expression value;
                if (_dict.TryGetValue(p.Name, out value))
                    return value;
                return base.VisitParameter(p);
            }
        }
    }
}