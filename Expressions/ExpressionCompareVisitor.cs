using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    public abstract class ExpressionCompareVisitor
    {
        //protected methods
        protected virtual bool Visit(Expression exp, Expression exp2)
        {
            if (exp2 == null)
                return exp == null;
            if (exp == null)
                return false;
            if (exp.NodeType != exp2.NodeType)
                return false;

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp, (UnaryExpression)exp2);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)exp, (BinaryExpression)exp2);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp, (TypeBinaryExpression)exp2);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp, (ConditionalExpression)exp2);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp, (ConstantExpression)exp2);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp, (ParameterExpression)exp2);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp, (MemberExpression)exp2);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp, (MethodCallExpression)exp2);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp, (LambdaExpression)exp2);
                case ExpressionType.New:
                    return VisitNew((NewExpression)exp, (NewExpression)exp2);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp, (NewArrayExpression)exp2);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp, (InvocationExpression)exp2);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp, (MemberInitExpression)exp2);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp, (ListInitExpression)exp2);
                default:
                    throw new ArgumentException(string.Format("Unhandled expression type: '{0}'", exp.NodeType), "exp");
            }
        }

        protected virtual bool VisitBinding(MemberBinding binding, MemberBinding binding2)
        {
            if (binding.Member != binding2.Member)
                return false;

            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding, (MemberAssignment)binding2);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding, (MemberMemberBinding)binding2);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding, (MemberListBinding)binding2);
                default:
                    throw new ArgumentException(string.Format("Unhandled binding type '{0}'", binding.BindingType), "binding");
            }
        }

        protected virtual bool VisitElementInitializer(ElementInit initializer, ElementInit initializer2)
        {
            return initializer.AddMethod == initializer2.AddMethod
                   && VisitExpressionList(initializer.Arguments, initializer2.Arguments);
        }

        protected virtual bool VisitUnary(UnaryExpression u, UnaryExpression u2)
        {
            return u.Type == u2.Type
                   && u.Method == u2.Method
                   && Visit(u.Operand, u2.Operand);
        }

        protected virtual bool VisitBinary(BinaryExpression b, BinaryExpression b2)
        {
            return b.Method == b2.Method
                   && b.IsLiftedToNull == b2.IsLiftedToNull
                   && Visit(b.Left, b2.Left)
                   && Visit(b.Right, b2.Right)
                   && Visit(b.Conversion, b2.Conversion);
        }

        protected virtual bool VisitTypeIs(TypeBinaryExpression b, TypeBinaryExpression b2)
        {
            return b.TypeOperand == b2.TypeOperand
                   && Visit(b.Expression, b2.Expression);
        }

        protected virtual bool VisitConstant(ConstantExpression c, ConstantExpression c2)
        {
            return Equals(c.Value, c2.Value);
        }

        protected virtual bool VisitConditional(ConditionalExpression c, ConditionalExpression c2)
        {
            return Visit(c.Test, c2.Test)
                   && Visit(c.IfTrue, c2.IfTrue)
                   && Visit(c.IfFalse, c2.IfFalse);
        }

        protected virtual bool VisitParameter(ParameterExpression p, ParameterExpression p2)
        {
            return p.Name == p2.Name
                   && p.Type == p2.Type;
        }

        protected virtual bool VisitMemberAccess(MemberExpression m, MemberExpression m2)
        {
            return m.Member == m2.Member
                   && Visit(m.Expression, m2.Expression);
        }

        protected virtual bool VisitMethodCall(MethodCallExpression m, MethodCallExpression m2)
        {
            return m.Method == m2.Method
                   && Visit(m.Object, m2.Object)
                   && VisitExpressionList(m.Arguments, m2.Arguments);
        }

        protected virtual bool VisitExpressionList(ReadOnlyCollection<Expression> original, ReadOnlyCollection<Expression> original2)
        {
            if (original.Count != original2.Count)
                return false;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (!Visit(original[i], original2[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool VisitMemberAssignment(MemberAssignment assignment, MemberAssignment assignment2)
        {
            return assignment.Member == assignment2.Member
                   && Visit(assignment.Expression, assignment2.Expression);
        }

        protected virtual bool VisitMemberMemberBinding(MemberMemberBinding binding, MemberMemberBinding binding2)
        {
            return binding.Member == binding2.Member
                   && VisitBindingList(binding.Bindings, binding2.Bindings);
        }

        protected virtual bool VisitMemberListBinding(MemberListBinding binding, MemberListBinding binding2)
        {
            return binding.Member == binding2.Member
                   && VisitElementInitializerList(binding.Initializers, binding2.Initializers);
        }

        protected virtual bool VisitBindingList(ReadOnlyCollection<MemberBinding> original, ReadOnlyCollection<MemberBinding> original2)
        {
            if (original.Count != original2.Count)
                return false;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (!VisitBinding(original[i], original2[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool VisitElementInitializerList(ReadOnlyCollection<ElementInit> original, ReadOnlyCollection<ElementInit> original2)
        {
            if (original.Count != original2.Count)
                return false;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (!VisitElementInitializer(original[i], original2[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool VisitLambda(LambdaExpression lambda, LambdaExpression lambda2)
        {
            return lambda.Type == lambda2.Type
                   && compareLambdaParameters(lambda.Parameters, lambda2.Parameters)
                   && Visit(lambda.Body, lambda2.Body);
        }

        protected virtual bool VisitNew(NewExpression nex, NewExpression nex2)
        {
            return nex.Constructor == nex2.Constructor
                   && compareNewMembers(nex.Members, nex2.Members)
                   && VisitExpressionList(nex.Arguments, nex2.Arguments);
        }

        protected virtual bool VisitMemberInit(MemberInitExpression init, MemberInitExpression init2)
        {
            return VisitNew(init.NewExpression, init2.NewExpression)
                   && VisitBindingList(init.Bindings, init2.Bindings);
        }

        protected virtual bool VisitListInit(ListInitExpression init, ListInitExpression init2)
        {
            return VisitNew(init.NewExpression, init2.NewExpression)
                   && VisitElementInitializerList(init.Initializers, init2.Initializers);
        }

        protected virtual bool VisitNewArray(NewArrayExpression na, NewArrayExpression na2)
        {
            return na.Type == na2.Type
                   && VisitExpressionList(na.Expressions, na2.Expressions);
        }

        protected virtual bool VisitInvocation(InvocationExpression iv, InvocationExpression iv2)
        {
            return VisitExpressionList(iv.Arguments, iv2.Arguments)
                   && Visit(iv.Expression, iv2.Expression);
        }

        //static methods
        static bool compareLambdaParameters(ReadOnlyCollection<ParameterExpression> original, ReadOnlyCollection<ParameterExpression> original2)
        {
            if (original.Count != original2.Count)
                return false;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (original[i].Name != original2[i].Name || original[i].Type != original2[i].Type)
                    return false;
            }
            return true;
        }

        static bool compareNewMembers(ReadOnlyCollection<MemberInfo> original, ReadOnlyCollection<MemberInfo> original2)
        {
            if (original == null)
                return original2 == null;
            if (original2 == null)
                return false;
            if (original.Count != original2.Count)
                return false;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (original[i] != original2[i])
                    return false;
            }
            return true;
        }
    }
}