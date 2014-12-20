using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Chaow.Expressions;
using Chaow.Extensions;

namespace Chaow.LINCON
{
    class ConstraintVar
    {
        internal Type Type { get; set; }
        internal bool IsList { get; set; }
        internal int Index { get; set; }
        internal int ListCount { get; set; }
    }

    public class ConstraintSolver<T> : IEnumerable<T>
    {
        //Variables
        static Expression zero = Expression.Constant(0);
        readonly Dictionary<string, Expression> _indexes = new Dictionary<string, Expression>();
        readonly List<ParameterExpression> _listParam = new List<ParameterExpression>();
        readonly MethodInfo[] _methods = new MethodInfo[3];
        readonly Dictionary<string, Expression> _paramFrom = new Dictionary<string, Expression>();
        readonly Dictionary<string, ParameterExpression> _paramIndex = new Dictionary<string, ParameterExpression>();
        readonly Dictionary<string, Expression> _paramTo = new Dictionary<string, Expression>();
        readonly Dictionary<string, ConstraintVar> _paramVar = new Dictionary<string, ConstraintVar>();
        readonly HashSet<ParameterExpression> _paramWhere = new HashSet<ParameterExpression>();
        readonly Expression _source;
        readonly Type _type;
        readonly List<ParameterExpression> _varParam = new List<ParameterExpression>();
        Expression _exp;
        bool _firstVariable;
        Expression _lateConstraint = zero;
        IEnumerable<T> _solutions;
        Type _type2;

        //Constructors
        internal ConstraintSolver(Type type, Expression source)
        {
            _type = type;
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Compile();
            return _solutions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Static Methods
        static Expression convert(Expression exp, Type type1, Type type2)
        {
            if (type1 != type2)
            {
                var p = Expression.Parameter(type1, "<>c");
                exp = Expression.Call(
                    typeof(Enumerable),
                    "Select",
                    new[] {type1, type2},
                    exp,
                    Expression.Lambda(
                        Expression.Convert(p, type2),
                        p
                        ));
            }
            return exp;
        }

        static Expression makeVarParam(ParameterExpression p, MethodInfo[] methods, int id, Type type2)
        {
            Expression exp = Expression.Call(
                Expression.Call(p, methods[1], zero),
                methods[0],
                Expression.Constant(id));
            if (exp.Type != type2)
                exp = Expression.Convert(exp, type2);
            return exp;
        }

        static Expression makeListParam(ParameterExpression p, MethodInfo[] method, int id, Type type1, Type type2)
        {
            Expression exp = Expression.Call(p, method[1], Expression.Constant(id));
            if (type1 != type2)
                exp = Expression.Call(
                    Expression.Call(
                        typeof(Enumerable),
                        "ToList",
                        new[] {type2},
                        convert(exp, type1, type2)),
                    "AsReadOnly", null);
            return exp;
        }

        //Local Methods
        void buildExpression(MethodCallExpression m)
        {
            if (m.Arguments[0].NodeType == ExpressionType.Call)
                buildExpression((MethodCallExpression)m.Arguments[0]);

            switch (m.Method.Name)
            {
                case "SelectMany":
                    selectMany(m);
                    break;
                case "Where":
                    where(m);
                    break;
                case "Select":
                    select(m);
                    break;
                default:
                    break;
            }
        }

        void selectMany(MethodCallExpression m)
        {
            var arg2 = (LambdaExpression)((UnaryExpression)m.Arguments[1]).Operand;
            var arg3 = (LambdaExpression)((UnaryExpression)m.Arguments[2]).Operand;

            if (!_firstVariable)
            {
                createVariable(arg2.Parameters[0], (MethodCallExpression)m.Arguments[0]);
                _firstVariable = true;
            }

            MethodCallExpression m2;
            if (arg2.Body.NodeType == ExpressionType.Constant)
            {
                var obj = ((ConstantExpression)arg2.Body).Value;
                m2 = (MethodCallExpression)obj.GetType().GetField("Expression", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
            }
            else
                m2 = (MethodCallExpression)flattenExpression(arg3.Parameters[0], arg2.Body);

            createVariable(arg3.Parameters[1], m2);
        }

        void createVariable(ParameterExpression p, MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "ToConstraintVar":
                    addConstraintVar(p, (ConstantExpression)m.Arguments[0]);
                    return;
                case "ToConstraintList":
                    addConstraintList(p,
                        (ConstantExpression)m.Arguments[0],
                        (ConstantExpression)m.Arguments[1]);
                    return;
                case "ToConstraintIndex":
                    addConstraintIndex(p, m.Arguments[0]);
                    return;
                default:
                    throw new InvalidOperationException("Cannot use nested constraint query");
            }
        }

        void addConstraintVar(ParameterExpression p, ConstantExpression domains)
        {
            _exp = Expression.Call(_exp, "AppendVariable", null, convert(domains, p.Type, _type));
            _paramVar.Add(p.Name, new ConstraintVar {Type = p.Type, IsList = false, Index = _varParam.Count});
            _paramWhere.Add(p);
            _paramTo.Add(p.Name, p);
            _varParam.Add(p);
        }

        void addConstraintList(ParameterExpression p, ConstantExpression domains, ConstantExpression count)
        {
            var elementType = p.Type.GetGenericArguments()[0];
            _exp = Expression.Call(_exp, "AppendList", null, convert(domains, elementType, _type), count);
            _paramVar.Add(p.Name, new ConstraintVar {Type = elementType, IsList = true, Index = _listParam.Count + 1, ListCount = (int)count.Value});
            _paramWhere.Add(p);
            _paramTo.Add(p.Name, p);
            _listParam.Add(p);
        }

        void addConstraintIndex(ParameterExpression p, Expression exp)
        {
            _indexes.Add(p.Name, exp);
            _paramIndex.Add(p.Name, p);
            _paramTo.Add(p.Name, p);
        }

        Expression flattenExpression(ParameterExpression p, Expression exp)
        {
            if (_paramTo.Count < 2)
                return exp;

            updateParams(p);
            foreach (var kvp in _paramTo)
                exp = exp.Replace(_paramFrom[kvp.Key], kvp.Value);
            return exp;
        }

        void updateParams(ParameterExpression param)
        {
            if (_paramTo.Count == _paramFrom.Count)
                return;

            _paramFrom.Clear();

            Expression exp = param;
            PropertyInfo[] props = null;
            for (var i = 1; i < _paramTo.Count; i++)
            {
                props = exp.Type.GetProperties();
                _paramFrom.Add(props[1].Name, Expression.Property(exp, props[1].Name));
                exp = Expression.Property(exp, props[0].Name);
            }
            _paramFrom.Add(props[0].Name, exp);
        }

        void where(MethodCallExpression m)
        {
            var arg2 = (LambdaExpression)((UnaryExpression)m.Arguments[1]).Operand;

            if (!_firstVariable)
            {
                createVariable(arg2.Parameters[0], (MethodCallExpression)m.Arguments[0]);
                _firstVariable = true;
            }

            addConstraint(flattenExpression(arg2.Parameters[0], arg2.Body));
        }

        void addConstraint(Expression exp)
        {
            //go out if solutions is empty (Enumerable.Empty<T>)
            if (_solutions != null)
                return;

            //split condition
            if (trySplitConstraint(exp))
                return;
            if (tryApplyIndex(exp))
                return;

            //replace parameter
            if (transformParameter(exp))
                return;

            if (_lateConstraint == zero)
                _lateConstraint = exp;
            else
                _lateConstraint = Expression.AndAlso(_lateConstraint, exp);
        }

        bool tryAppendAllDifferent(ReadOnlyCollection<Expression> exps)
        {
            var list = new HashSet<long>();
            foreach (var exp in exps)
            {
                switch (exp.NodeType)
                {
                    case ExpressionType.Call:
                        var m = (MethodCallExpression)exp;
                        if (m.Method.Name == "get_Item" && m.Object.NodeType == ExpressionType.Parameter && m.Arguments[0].NodeType == ExpressionType.Constant)
                            list.Add(IntExt.CreateLong(_paramVar[((ParameterExpression)m.Object).Name].Index, (int)((ConstantExpression)m.Arguments[0]).Value));
                        else
                            return false;
                        break;
                    case ExpressionType.Parameter:
                        list.Add(IntExt.CreateLong(0, _paramVar[((ParameterExpression)exp).Name].Index));
                        break;
                    default:
                        return false;
                }
            }
            _exp = Expression.Call(_exp, "AppendAllDifferent", null, Expression.Constant(list));
            return true;
        }

        bool tryAddAllDifferent(MethodCallExpression m)
        {
            if (m.Method.Name != "AllDifferent" || m.Method.DeclaringType != typeof(Constraint))
                return false;
            if (tryApplyIndex(m))
                return true;

            ReadOnlyCollection<Expression> exps;
            ConstantExpression comparer = null;

            switch (m.Arguments.Count)
            {
                case 0:
                    return true;
                case 1:
                case 2:
                    if (m.Arguments[0].Type.IsArray)
                    {
                        if (m.Arguments[0].NodeType == ExpressionType.NewArrayInit)
                            exps = ((NewArrayExpression)m.Arguments[0]).Expressions;
                        else
                            return false;
                        if (m.Arguments.Count == 2)
                            comparer = (ConstantExpression)m.Arguments[1];
                    }
                    else if (m.Arguments[0].Type.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
                    {
                        if (m.Arguments[0].NodeType == ExpressionType.Parameter)
                        {
                            var param = (ParameterExpression)m.Arguments[0];
                            var method = param.Type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
                            exps = new ReadOnlyCollection<Expression>(
                                0.To(_paramVar[param.Name].ListCount - 1).Select(i =>
                                    (Expression)Expression.Call(param, method, Expression.Constant(i))
                                    ).ToList());
                        }
                        else
                            return false;
                        if (m.Arguments.Count == 2)
                            comparer = (ConstantExpression)m.Arguments[1];
                    }
                    else if (m.Arguments.Count == 1)
                        return true;
                    else
                        exps = m.Arguments;
                    break;
                default:
                    exps = m.Arguments;
                    break;
            }

            if (comparer == null)
            {
                if (tryAppendAllDifferent(exps))
                    return true;

                var type = typeof(EqualityComparer<>);
                type = type.MakeGenericType(exps[0].Type);
                comparer = Expression.Constant(
                    type.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null, null));
            }
            for (var i = 0; i < exps.Count; i++)
            {
                for (var j = i + 1; j < exps.Count; j++)
                    addConstraint(Expression.Not(Expression.Call(comparer, "Equals", null, exps[i], exps[j])));
            }
            return true;
        }

        bool trySplitConstraint(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    var and = (BinaryExpression)exp;
                    addConstraint(and.Left);
                    addConstraint(and.Right);
                    return true;
                case ExpressionType.Call:
                    if (tryAddAllDifferent((MethodCallExpression)exp))
                        return true;
                    return false;
                case ExpressionType.Conditional:
                    var cond = (ConditionalExpression)exp;
                    if (cond.Test.NodeType == ExpressionType.Constant)
                    {
                        addConstraint(((bool)((ConstantExpression)cond.Test).Value) ? cond.IfTrue : cond.IfFalse);
                        return true;
                    }
                    if (cond.IfTrue.NodeType != ExpressionType.Constant && cond.IfFalse.NodeType != ExpressionType.Constant)
                    {
                        addConstraint(Expression.Condition(cond.Test, cond.IfTrue, Expression.Constant(true)));
                        addConstraint(Expression.Condition(cond.Test, Expression.Constant(true), cond.IfFalse));
                        return true;
                    }
                    return false;
                case ExpressionType.Constant:
                    var b = (bool)((ConstantExpression)exp).Value;
                    if (!b)
                        _solutions = Enumerable.Empty<T>();
                    return true;
                default:
                    return false;
            }
        }

        bool tryApplyIndex(Expression exp)
        {
            var list = new List<string>();
            foreach (var id in _paramIndex)
            {
                if (exp.Contains(id.Value))
                    list.Add(id.Value.Name);
            }
            if (list.Count > 0)
            {
                applyIndex(exp, list, new KeyValuePair<string, int>[0]);
                return true;
            }
            return false;
        }

        void applyIndex(Expression exp, List<string> list, KeyValuePair<string, int>[] dict)
        {
            if (list.Count == dict.Length)
            {
                foreach (var kvp in dict)
                    exp = exp.Replace(_paramIndex[kvp.Key], Expression.Constant(kvp.Value));
                addConstraint(exp.Evaluate(e =>
                {
                    if (e.NodeType != ExpressionType.Call)
                        return false;
                    var m = ((MethodCallExpression)e).Method;
                    return m.Name == "Protect" && m.DeclaringType == typeof(Constraint);
                }));
                return;
            }

            var id = list[dict.Length];
            var param = _indexes[id];
            if (param.NodeType != ExpressionType.Constant)
            {
                foreach (var kvp in dict)
                    param = param.Replace(_paramIndex[kvp.Key], Expression.Constant(kvp.Value));
                param = param.Evaluate();
                if (param.NodeType != ExpressionType.Constant)
                {
                    foreach (var kvp in _paramIndex)
                        param = param.Replace(kvp.Value, Expression.Constant(0));
                    param = param.Evaluate();
                    if (param.NodeType != ExpressionType.Constant)
                        throw new InvalidOperationException("ConstraintIndex cannot contains ConstraintVar or ConstraintList");
                }
            }
            var source = (IEnumerable<int>)((ConstantExpression)param).Value;
            foreach (var i in source)
                applyIndex(exp, list, dict.Append(new KeyValuePair<string, int>(id, i)));
        }

        bool transformParameter(Expression exp)
        {
            Expression exp2;
            HashSet<long> paramId;
            var p = Expression.Parameter(_type2, "<>p");
            if (!new ParamTransformer().Transform(_paramWhere, _paramVar, p, _methods, exp, out exp2, out paramId))
                return false;

            if (paramId.Count == 1)
                _exp = Expression.Call(_exp, "AppendNodeConstraint", null, Expression.Constant(paramId.First()), exp2);
            else
                _exp = Expression.Call(_exp, "AppendArcConstraint", null, Expression.Constant(paramId), exp2);
            return true;
        }

        void select(MethodCallExpression m)
        {
            var arg2 = (LambdaExpression)((UnaryExpression)m.Arguments[1]).Operand;

            if (!_firstVariable)
            {
                createVariable(arg2.Parameters[0], (MethodCallExpression)m.Arguments[0]);
                _firstVariable = true;
            }

            if (_paramTo.Count >= 2)
                updateParams(arg2.Parameters[0]);
            _exp = Expression.Call(_exp, "LookAhead", null);

            var list = new Type[2];
            var method = new MethodInfo[2];
            list[0] = typeof(ReadOnlyCollection<>).MakeGenericType(_type);
            list[1] = typeof(List<>).MakeGenericType(list[0]);
            method[0] = list[0].GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
            method[1] = list[1].GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);

            if (_lateConstraint != zero)
            {
                _lateConstraint = convertSelect(_paramTo, _lateConstraint, Expression.Parameter(list[1], "<>p"), method);
                _exp = Expression.Call(typeof(Enumerable), "Where", new[] {list[1]}, _exp, _lateConstraint);
            }

            var targetType = arg2.Body.Type;
            var select = convertSelect(_paramFrom, arg2.Body, Expression.Parameter(list[1], "<>p"), method);
            _exp = Expression.Call(typeof(Enumerable), "Select", new[] {list[1], targetType}, _exp, select);
        }

        Expression convertSelect(Dictionary<string, Expression> dict, Expression exp, ParameterExpression p, MethodInfo[] method)
        {
            foreach (var kvp in dict)
            {
                if (!_paramIndex.ContainsKey(kvp.Key))
                {
                    var conVar = _paramVar[kvp.Key];
                    if (conVar.IsList)
                        exp = exp.Replace(kvp.Value,
                            makeListParam(p, method, conVar.Index, _type, conVar.Type));
                    else
                        exp = exp.Replace(kvp.Value,
                            makeVarParam(p, method, conVar.Index, conVar.Type));
                }
            }
            return Expression.Lambda(exp, p);
        }

        //Public Methods
        public void Compile()
        {
            //Exit if built
            if (_solutions != null)
                return;

            //Create solver
            _exp = Expression.New(typeof(LookAheadSolver<>).MakeGenericType(_type));

            //prepare
            _type2 = typeof(List<>).MakeGenericType(_type);
            _methods[0] = _type2.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
            _type2 = typeof(List<>).MakeGenericType(_type2);
            _methods[1] = _type2.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
            _type2 = typeof(List<>).MakeGenericType(_type2);
            _methods[2] = _type2.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);

            //build
            buildExpression((MethodCallExpression)_source);
            if (_solutions == null)
                _solutions = (IEnumerable<T>)Expression.Lambda(_exp).Compile().DynamicInvoke(null);
        }
    }

    class ParamTransformer : ExpressionVisitor
    {
        static readonly Expression zero = Expression.Constant(0);
        readonly HashSet<long> _paramId = new HashSet<long>();
        bool _indexOutOfLength;
        MethodInfo[] _methods;
        bool _nonConstant;
        ParameterExpression _param;
        Dictionary<string, ConstraintVar> _paramVar;
        HashSet<ParameterExpression> _paramWhere;

        internal bool Transform(HashSet<ParameterExpression> paramWhere, Dictionary<string, ConstraintVar> paramVar, ParameterExpression param, MethodInfo[] methods, Expression exp, out Expression exp2, out HashSet<long> paramId)
        {
            _nonConstant = false;
            _indexOutOfLength = false;
            _paramId.Clear();
            _paramWhere = paramWhere;
            _paramVar = paramVar;
            _param = param;
            _methods = methods;
            exp2 = Expression.Lambda(Visit(exp), param);
            paramId = _paramId;
            return !_nonConstant;
        }

        public override Expression Visit(Expression exp)
        {
            if (_nonConstant || _indexOutOfLength)
                return exp;

            var tmp = base.Visit(exp);

            if (_indexOutOfLength && tmp.Type == typeof(bool))
            {
                _indexOutOfLength = false;
                tmp = Expression.Constant(false);
            }

            return tmp;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_paramWhere.Contains(p) && !_paramVar[p.Name].IsList)
            {
                var kvp = IntExt.CreateLong(0, _paramVar[p.Name].Index);
                if (!_paramId.Contains(kvp))
                    _paramId.Add(kvp);
                return makeVarParam(_param, _methods, kvp.GetLowInt(), p.Type);
            }
            return p;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object != null && m.Object.NodeType == ExpressionType.Parameter)
            {
                var p = (ParameterExpression)m.Object;
                if (_paramWhere.Contains(p))
                {
                    if (m.Method.Name != "get_Item" || m.Arguments[0].NodeType != ExpressionType.Constant)
                    {
                        _nonConstant = true;
                        return m;
                    }
                    var paramVar = _paramVar[p.Name];
                    var index = (int)((ConstantExpression)m.Arguments[0]).Value;
                    if (index < 0 || index >= paramVar.ListCount)
                    {
                        _indexOutOfLength = true;
                        return m;
                    }
                    var kvp = IntExt.CreateLong(paramVar.Index, index);
                    if (!_paramId.Contains(kvp))
                        _paramId.Add(kvp);
                    return makeListParam(_param, _methods, kvp, m.Type);
                }
            }
            return base.VisitMethodCall(m);
        }

        static Expression makeVarParam(ParameterExpression p, MethodInfo[] methods, int id, Type type2)
        {
            Expression exp = Expression.Call(
                Expression.Call(
                    Expression.Call(p, methods[2], zero),
                    methods[1],
                    Expression.Constant(id)),
                methods[0],
                zero);
            if (exp.Type != type2)
                exp = Expression.Convert(exp, type2);
            return exp;
        }

        static Expression makeListParam(ParameterExpression p, MethodInfo[] methods, long kvp, Type type2)
        {
            Expression exp = Expression.Call(
                Expression.Call(
                    Expression.Call(p, methods[2], Expression.Constant(kvp.GetHighInt())),
                    methods[1],
                    Expression.Constant(kvp.GetLowInt())),
                methods[0],
                zero);
            if (exp.Type != type2)
                exp = Expression.Convert(exp, type2);
            return exp;
        }
    }
}