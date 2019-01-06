using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox
{
    public class AstPrinter : Expression.Visitor<string>
    {
        public string Print(Expression expression)
        {
            return expression.Accept(this);
        }

        public string VisitBinary(Expression.Binary expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
        }

        public string VisitGrouping(Expression.Grouping expression)
        {
            return Parenthesize("grouping", expression.Expression);
        }

        public string VisitLiteral(Expression.Literal expression)
        {
            if (expression.Value == null)
            {
                return "nil";
            }
            return expression.Value.ToString();
        }

        public string VisitUnary(Expression.Unary expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Right);
        }

        public string VisitVariable(Expression.Variable expression)
        {
            return expression.Name.Lexeme;
        }

        private string Parenthesize(string name, params Expression[] expressions)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("(")
                .Append(name);

            foreach(var expression in expressions)
            {
                stringBuilder.Append(" ")
                    .Append(expression.Accept(this));
            }

            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
    }
}
