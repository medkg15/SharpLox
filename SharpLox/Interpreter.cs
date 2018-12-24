using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Interpreter : Expression.Visitor<object>
    {
        public object VisitBinary(Expression.Binary expression)
        {
            var left = Evaluate(expression.Left);
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    return (double)left - (double)right;
                case TokenType.Slash:
                    return (double)left / (double)right;
                case TokenType.Star:
                    return (double)left * (double)right;
                case TokenType.Plus:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    break;
                case TokenType.Greater:
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    return (double)left >= (double)right;
                case TokenType.Less:
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    return (double)left <= (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.Equal:
                    return IsEqual(left, right);
            }
        }

        public object VisitGrouping(Expression.Grouping expression)
        {
            return Evaluate(expression.Expression);
        }

        public object VisitLiteral(Expression.Literal expression)
        {
            return expression.Value;
        }

        public object VisitUnary(Expression.Unary expression)
        {
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    return -(double)right;
                case TokenType.Bang:
                    return !IsTruthy(right);
            }

            // unreachable.
            return null;
        }

        private object Evaluate(Expression expression)
        {
            return expression.Accept(this);
        }

        private bool IsEqual(object a, object b)
        {
            // nil is only equal to nil.               
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is bool boolVal)
            {
                return boolVal;
            }
            return true;
        }
    }
}