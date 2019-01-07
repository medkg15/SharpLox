using System;
using System.Collections.Generic;

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
			T VisitVariable(Variable expression);
			T VisitAssign(Assign expression);
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

		public class Variable : Expression
		{
			public Variable(
				Token name
			)
			{
				this.Name = name;
			}

			public Token Name { get; set; }

			public override T Accept<T>(Visitor<T> visitor)
			{
				return visitor.VisitVariable(this);
			}
		} 

		public class Assign : Expression
		{
			public Assign(
				Token name,
				Expression value
			)
			{
				this.Name = name;
				this.Value = value;
			}

			public Token Name { get; set; }
			public Expression Value { get; set; }

			public override T Accept<T>(Visitor<T> visitor)
			{
				return visitor.VisitAssign(this);
			}
		} 
	}

	public abstract class Statement
	{
		public interface Visitor
		{
			void VisitExpressionStatement(ExpressionStatement statement);
			void VisitPrintStatement(PrintStatement statement);
			void VisitVarStatement(VarStatement statement);
			void VisitBlock(Block statement);
		}

		public abstract void Accept(Visitor visitor);

		public class ExpressionStatement : Statement
		{
			public ExpressionStatement(
				Expression expression
			)
			{
				this.Expression = expression;
			}

			public Expression Expression { get; set; }

			public override void Accept(Visitor visitor)
			{
				visitor.VisitExpressionStatement(this);
			}
		} 

		public class PrintStatement : Statement
		{
			public PrintStatement(
				Expression expression
			)
			{
				this.Expression = expression;
			}

			public Expression Expression { get; set; }

			public override void Accept(Visitor visitor)
			{
				visitor.VisitPrintStatement(this);
			}
		} 

		public class VarStatement : Statement
		{
			public VarStatement(
				Token name,
				Expression initializer
			)
			{
				this.Name = name;
				this.Initializer = initializer;
			}

			public Token Name { get; set; }
			public Expression Initializer { get; set; }

			public override void Accept(Visitor visitor)
			{
				visitor.VisitVarStatement(this);
			}
		} 

		public class Block : Statement
		{
			public Block(
				List<Statement> statements
			)
			{
				this.Statements = statements;
			}

			public List<Statement> Statements { get; set; }

			public override void Accept(Visitor visitor)
			{
				visitor.VisitBlock(this);
			}
		} 
	} 
} 