using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using Chaow.Expressions;

namespace Chaow.LINCON
{
    public static class Constraint
    {
        public static ConstraintSelect<T> ToConstraintVar<T>(this IEnumerable<T> source)
        {
            var exp = Expression.Call(null,
                ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                Expression.Constant(source));
            return new ConstraintSelect<T>(exp, typeof(T));
        }

        public static ConstraintSelect<ReadOnlyCollection<T>> ToConstraintList<T>(this IEnumerable<T> source, int count)
        {
            var exp = Expression.Call(null,
                ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                Expression.Constant(source),
                Expression.Constant(count));
            return new ConstraintSelect<ReadOnlyCollection<T>>(exp, typeof(T));
        }

        public static ConstraintSelect<int> ToConstraintIndex(this IEnumerable<int> source)
        {
            var exp = Expression.Call(null,
                (MethodInfo)MethodBase.GetCurrentMethod(),
                Expression.Constant(source));
            return new ConstraintSelect<int>(exp, null);
        }

        public static ConstraintSelect<V> SelectMany<T, U, V>(this ConstraintSelect<T> first, Expression<Func<T, ConstraintSelect<U>>> second, Expression<Func<T, U, V>> selector)
        {
            second = second.Evaluate();
            Type type = null;
            if (second.Body.NodeType == ExpressionType.Constant)
            {
                var secondObj = (ConstraintSelect<U>)((ConstantExpression)second.Body).Value;
                type = secondObj.Type;
            }

            var exp = Expression.Call(null,
                ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T), typeof(U), typeof(V)),
                first.Expression,
                Expression.Quote(second),
                Expression.Quote(selector));
            return new ConstraintSelect<V>(exp, mergeType(first.Type, type));
        }

        public static ConstraintWhere<T> Where<T>(this Constraint<T> source, Expression<Func<T, bool>> predicate)
        {
            var exp = Expression.Call(null,
                ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                source.Expression,
                Expression.Quote(predicate));
            return new ConstraintWhere<T>(exp, source.Type);
        }

        public static ConstraintSolver<U> Select<T, U>(this Constraint<T> source, Expression<Func<T, U>> selector)
        {
            var exp = Expression.Call(null,
                ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T), typeof(U)),
                source.Expression,
                Expression.Quote(selector));
            var constraint = new Constraint<U>(exp, source.Type);
            return constraint.CreateSolver<U>();
        }

        public static bool AllDifferent<T>(ReadOnlyCollection<T> members)
        {
            return AllDifferent(members, EqualityComparer<T>.Default);
        }

        public static bool AllDifferent<T>(params T[] members)
        {
            return AllDifferent(members, EqualityComparer<T>.Default);
        }

        public static bool AllDifferent<T>(IList<T> members, IEqualityComparer<T> comparer)
        {
            var len = members.Count;

            for (var i = 0; i < len; i++)
            {
                for (var j = i + 1; j < len; j++)
                {
                    if (comparer.Equals(members[i], members[j]))
                        return false;
                }
            }
            return true;
        }

        static Type mergeType(Type type1, Type type2)
        {
            if (type1 == type2)
                return type1;
            if (type2 == null)
                return type1;
            if (type1 == null)
                return type2;
            if (type2.IsEnum)
                type2 = Enum.GetUnderlyingType(type2);
            if (type1.IsEnum)
                type1 = Enum.GetUnderlyingType(type1);

            var code = Math.Max(getTypeCode(type1), getTypeCode(type2));
            switch (code)
            {
                case 1:
                    return typeof(int);
                case 2:
                    return typeof(long);
                case 3:
                    return typeof(decimal);
                default:
                    return typeof(object);
            }
        }

        static int getTypeCode(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                    return 1;
                case TypeCode.UInt32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 3;
                default:
                    return int.MaxValue;
            }
        }

        public static T Protect<T>(this T source)
        {
            return source;
        }
    }

    public class Constraint<T>
    {
        internal readonly Expression Expression;
        internal readonly Type Type;

        internal Constraint(Expression exp, Type type)
        {
            Expression = exp;
            Type = type;
        }

        internal ConstraintSolver<U> CreateSolver<U>()
        {
            return new ConstraintSolver<U>(Type, Expression);
        }
    }

    public class ConstraintSelect<T> : Constraint<T>
    {
        internal ConstraintSelect(Expression exp, Type type) : base(exp, type)
        {
        }
    }

    public class ConstraintWhere<T> : Constraint<T>
    {
        internal ConstraintWhere(Expression exp, Type type) : base(exp, type)
        {
        }
    }
}