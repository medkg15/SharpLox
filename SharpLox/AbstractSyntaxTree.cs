using System;

namespace SharpLox
{
	public abstract class Expression
	{
		public interface Visitor<T>
		{
			T VisitBinary(Binary expression);
			T VisitGrouping(Grouping expression);
			T VisitLiteral(Literal expression);
			T VisitUnary(Unary expression);
		}

		public abstract T Accept<T>(Visitor<T> visitor);

		public class Binary : Expression
		{
			public Binary(
				Expression left,
				Token @operator,
				Expression right
			)
			{
				this.Left = left;
				this.Operator = @operator;
				this.Right = right;
			}

			public Expression Left { get; set; }
			public Token Operator { get; set; }
			public Expression Right { get; set; }

			public override T Accept<T>(Visitor<T> visitor)
			{
				return visitor.VisitBinary(this);
			}
		} 

		public class Grouping : Expression
		{
			public Grouping(
				Expression expression
			)
			{
				this.Expression = expression;
			}

			public Expression Expression { get; set; }

			public override T Accept<T>(Visitor<T> visitor)
			{
				return visitor.VisitGrouping(this);
			}
		} 

		public class Literal : Expression
		{
			public Literal(
				Object value
			)
			{
				this.Value = value;
			}

			public Object Value { get; set; }

			public override T Accept<T>(Visitor<T> visitor)
			{
				return visitor.VisitLiteral(this);
			}
		} 

		public class Unary : Expression
		{
			public Unary(
				Token @operator,
				Expression right
			)
			{
				this.Operator = @operator;
				this.Right = right;
			}

			public Token Operator { get; set; }
			public Expression Right { get; set; }

			public override T Accept<T>(Visitor<T> visitor)
			{
				return visitor.VisitUnary(this);
			}
		} 
	} 
} 