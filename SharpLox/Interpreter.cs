using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Interpreter : Expression.Visitor<object>, Statement.Visitor
    {
        private Environment _environment = new Environment();

        public void Interpret(List<Statement> statements)
        {
            try
            {
                foreach(var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch(RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private void Execute(Statement statement)
        {
            statement.Accept(this);
        }
        
        private object Evaluate(Expression expression)
        {
            return expression.Accept(this);
        }

        public void VisitExpressionStatement(Statement.ExpressionStatement statement)
        {
            Evaluate(statement.Expression);
        }

        public void VisitPrintStatement(Statement.PrintStatement statement)
        {
            var value = Evaluate(statement.Expression);
            Console.WriteLine(Stringify(value));
        }

        public void VisitVarStatement(Statement.VarStatement statement)
        {
            object value = null;

            if (statement.Initializer != null)
            {
                value = Evaluate(statement.Initializer);
            }

            _environment.Define(statement.Name.Lexeme, value);
        }

        public object VisitBinary(Expression.Binary expression)
        {
            var left = Evaluate(expression.Left);
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.Slash:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    CheckNumberOperands(expression.Operator, left, right);
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
                    throw new RuntimeError(expression.Operator, "Operands must be two numbers or two strings.");
                case TokenType.Greater:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);
                default:
                    throw new NotImplementedException();
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
                    CheckNumberOperand(expression.Operator, right);
                    return -(double)right;
                case TokenType.Bang:
                    return !IsTruthy(right);
            }

            // unreachable.
            return null;
        }

        public object VisitVariable(Expression.Variable expression)
        {
            return _environment.Get(expression.Name);
        }

        public object VisitAssign(Expression.Assign expression)
        {
            var value = Evaluate(expression.Value);
            _environment.Assign(expression.Name, value);
            return value;
        }

        private void CheckNumberOperand(Token @operator, object right)
        {
            if (right is double)
            {
                return;
            }

            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (right is double && left is double)
            {
                return;
            }

            throw new RuntimeError(@operator, "Operands must be numbers.");
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

        private string Stringify(object obj)
        {
            if (obj == null)
            {
                return "nil";
            }

            return obj.ToString();
        }
    }
}