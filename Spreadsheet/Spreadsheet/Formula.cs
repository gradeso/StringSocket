using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SS
{
	/// <summary>
	/// Represents formulas written in standard infix notation using standard precedence
	/// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
	/// non-negative floating-point numbers, variables, left and right parentheses, and
	/// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
	/// are not allowed.)
	/// </summary>
	public struct Formula
	{
		/// <summary>
		/// Creates a Formula from a string that consists of a standard infix expression composed
		/// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
		/// variable symbols (a letter followed by zero or more letters and/or digits), left and right
		/// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
		/// permitted between tokens, but is not required.
		/// 
		/// Examples of a valid parameter to this constructor are:
		///     "2.5e9 + x5 / 17"
		///     "(5 * 2) + 8"
		///     "x*y-2+35/9"
		///     
		/// Examples of invalid parameters are:
		///     "_"
		///     "-5.3"
		///     "2 5 + 3"
		/// 
		/// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
		/// explanatory Message.
		/// </summary>
		/// 
		private const char varKey = '`';

		private const char numKey = '&';

		private List<KeyValuePair<string, char>> keyList;




		//main constructor
		public Formula(String formula, Normalizer N, Validator V)
		{
			keyList = new List<KeyValuePair<string, char>>();
			checkFormula(formula);
			var templist = new List<KeyValuePair<string, char>>();
			foreach (KeyValuePair<string, char> kvp in keyList)
			{
				if (kvp.Value == varKey)
				{
					string normalized = N(kvp.Key);
					if (V(normalized))
					{
						templist.Add(new KeyValuePair<string, char>(normalized, varKey));

					}
					else
					{
						throw new FormulaFormatException("validator failed");
					}
				}
				else
				{
					templist.Add(kvp);
				}
			}
			keyList = templist;
		}
		//constructor
		public Formula(String formula) : this(formula, (s => s), (s => true))
		{



		}
		//checks if given formula is valid.
		private void checkFormula(string formula)
		{
			//ampersand signifies a number or variable being needed in next token.
			//a * will signify operators
			string nextCondition = "(&";
			int openParenCount = 0;
			int closedParenCount = 0;
			bool isNotWhitespace = false;
			//itterate through th formula
			foreach (string s in GetTokens(formula))
			{
				//and use pass by reference to keep trak of multiple varliables.
				operate(ref isNotWhitespace, ref nextCondition, ref openParenCount, ref closedParenCount, s);
				if (closedParenCount > openParenCount)
				{
					throw new FormulaFormatException("unmatched parentheses");

				}
			}
			if (closedParenCount != openParenCount)
			{
				throw new FormulaFormatException("unmatched parentheses");

			}
			if (nextCondition.Contains("(&"))
			{
				throw new FormulaFormatException("unmatched operator");

			}
			if (!isNotWhitespace) throw new FormulaFormatException("need more then just space");

		}

		//method that updates the variables in check formula and serves as the main opperating method.
		private void operate(ref bool isNotWhitespace, ref string nextCondition, ref int openParenCount, ref int closedParenCount, string s)
		{
			//chuck whitespace
			if (string.IsNullOrWhiteSpace(s)) return;
			if (s.Length == 1)
			{
				if ("+-/*".Contains(s))
				{
					switch (s)
					{
						case "+":
							keyList.Add(new KeyValuePair<string, char>(s, '+'));
							break;
						case "/":
							keyList.Add(new KeyValuePair<string, char>(s, '/'));
							break;

						case "*":
							keyList.Add(new KeyValuePair<string, char>(s, '*'));
							break;

						case "-":
							keyList.Add(new KeyValuePair<string, char>(s, '-'));
							break;
						default: break;

					}
					if (nextCondition.Contains("*"))
					{
						nextCondition = "(&";
						return;
					}
					else
					{
						throw new FormulaFormatException("nonsense input");
					}
				}
				else if (")".Equals(s))
				{
					keyList.Add(new KeyValuePair<string, char>(s, ')'));
					if (nextCondition.Contains(")"))
					{
						closedParenCount++;
						nextCondition = "*)";
						return;
					}
					else
					{
						throw new FormulaFormatException("nonsense input");
					}
				}
				else if ("(".Equals(s))
				{
					keyList.Add(new KeyValuePair<string, char>(s, '('));
					if (nextCondition.Contains("("))
					{
						isNotWhitespace = true;
						openParenCount++;
						nextCondition = "(&";
						return;
					}
					else
					{
						throw new FormulaFormatException("nonsense input");
					}
				}
			}
			//if num
			double bs;
			if (double.TryParse(s, out bs))
			{
				keyList.Add(new KeyValuePair<string, char>(s, numKey));
				if (nextCondition.Contains("&"))
				{
					isNotWhitespace = true;
					nextCondition = "*)";
					return;
				}
				else
				{
					throw new FormulaFormatException("nonsense input");
				}
			}
			//if var
			if ("abcdefghigklmnopqrstuvwxyz".Contains(s.Substring(0, 1).ToLower()))
			{
				keyList.Add(new KeyValuePair<string, char>(s, varKey));
				if (nextCondition.Contains("&"))
				{
					isNotWhitespace = true;
					nextCondition = "*)";
					return;
				}
				else
				{
					throw new FormulaFormatException("nonsense input");
				}
			}
			throw new FormulaFormatException("Token was unrecognizeable");
		}


		/// <summary>
		/// checks if there was an op token previously in the enumerable.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>


		/// <summary>
		/// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
		/// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
		/// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
		/// 
		/// If no undefined variables or divisions by zero are encountered when evaluating 
		/// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
		/// with an explanatory Message.
		/// </summary>
		public double Evaluate(Lookup lookup)
		{
			fixDefualtConstructor();

			var opStack = new Stack<string>();
			var valStack = new Stack<double>();
			var en = keyList.GetEnumerator();
			while (en.MoveNext())
			{
				stackShimmy(en.Current, lookup, ref opStack, ref valStack);
			}
			if (opStack.Count == 0)
			{
				return valStack.Pop();
			}
			else
			{
				double temp1 = valStack.Pop();
				double temp2 = valStack.Pop();
				return mathamatize(temp2, temp1, opStack.Pop());
			}
		}
		//moves around the stacks based on the type of token.
		private void stackShimmy(KeyValuePair<string, char> token, Lookup lookup, ref Stack<string> opStack, ref Stack<double> valStack)
		{
			//local variables for temporary use

			string opPop;
			double temp1 = 0;
			double temp2 = 0;
			switch (token.Value)
			{
				case '*':
				case '/':
				case '(':
					opStack.Push(token.Key);
					break;

				case '+':
				case '-':
					if (opStack.Count > 0)
					{
						if (opStack.Peek() == "+" || opStack.Peek() == "-")
						{
							opPop = opStack.Pop();
							temp1 = valStack.Pop();
							temp2 = valStack.Pop();
							valStack.Push(mathamatize(temp2, temp1, opPop));

						}
					}
					opStack.Push(token.Key);
					break;

				case ')':

					if (opStack.Peek() == "+" || opStack.Peek() == "-")
					{
						opPop = opStack.Pop();
						temp1 = valStack.Pop();
						temp2 = valStack.Pop();
						valStack.Push(mathamatize(temp2, temp1, opPop));
					}


					//gets rid of closing paren.
					opStack.Pop();
					if (opStack.Count > 0)
					{
						if (opStack.Peek() == "/" || opStack.Peek() == "*")
						{
							opPop = opStack.Pop();
							temp1 = valStack.Pop();
							temp2 = valStack.Pop();
							valStack.Push(mathamatize(temp2, temp1, opPop));
						}
					}
					break;


				case varKey:
					try
					{
						temp1 = lookup(token.Key);
					}
					catch (Exception)
					{
						throw new FormulaEvaluationException("bad lookup for variables");
					}
					if (opStack.Count > 0)
					{
						if (opStack.Peek() == "/" || opStack.Peek() == "*")
						{
							temp2 = valStack.Pop();
							opPop = opStack.Pop();
							valStack.Push(mathamatize(temp2, temp1, opPop));
						}
						else
						{
							valStack.Push(temp1);
						}
					}
					else
					{
						valStack.Push(temp1);
					}
					break;
				case numKey:
					temp1 = Double.Parse(token.Key);
					if (opStack.Count > 0)
					{
						if (opStack.Peek() == "/" || opStack.Peek() == "*")
						{
							temp2 = valStack.Pop();
							opPop = opStack.Pop();
							valStack.Push(mathamatize(temp2, temp1, opPop));
						}
						else
						{
							valStack.Push(temp1);
						}
					}
					else
					{
						valStack.Push(temp1);
					}
					break;

			}

		}
		//does individual operations
		private double mathamatize(double p1, double p2, string op)
		{
			switch (op)
			{
				case "+":
					return p1 + p2;
				case "-":
					return p1 - p2;
				case "/":
					if (p2 == 0) throw new FormulaEvaluationException("divide by 0");
					return p1 / p2;
				case "*":
					return p1 * p2;
				default:
					throw new FormulaEvaluationException("opppeator clog");
			}
		}


		/// <summary>
		/// Gets the variables.
		/// </summary>
		/// <returns></returns>
		public ISet<string> GetVariables()
		{
			fixDefualtConstructor();
			var toReturn = new HashSet<string>();
			var en = keyList.GetEnumerator();
			while (en.MoveNext())
			{
				if (en.Current.Value == varKey)
				{
					toReturn.Add(en.Current.Key);
				}
			}
			return toReturn;
		}
		/// <summary>
		/// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
		/// right paren, one of the four operator symbols, a string consisting of a letter followed by
		/// zero or more digits and/or letters, a double literal, and anything that doesn't
		/// match one of those patterns.  There are no empty tokens, and no token contains white space.
		/// </summary>
		private static IEnumerable<string> GetTokens(String formula)
		{

			// Patterns for individual tokens
			String lpPattern = @"\(";
			String rpPattern = @"\)";
			String opPattern = @"[\+\-*/]";
			String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
			// PLEASE NOTE:  I have added white space to this regex to make it more readable.
			// When the regex is used, it is necessary to include a parameter that says
			// embedded white space should be ignored.  See below for an example of this.
			String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
			String spacePattern = @"\s+";

			// Overall pattern.  It contains embedded white space that must be ignored when
			// it is used.  See below for an example of this.
			String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
											lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

			// Enumerate matching tokens that don't consist solely of white space.
			// PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
			/// in the pattern.
			foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
			{
				if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
				{
					yield return s;
				}
			}
		}

		/// <summary>
		/// allows the formua to be string in normalized form
		/// </summary>
		/// <returns></returns>

		public override string ToString()
		{
			fixDefualtConstructor();

			string toReturn = "";
			foreach (KeyValuePair<string, char> kvp in keyList)
			{
				toReturn = toReturn + kvp.Key;
			}
			return toReturn;
		}
		private void fixDefualtConstructor()
		{
			if (keyList == null)
			{
				keyList = new List<KeyValuePair<string, char>>();

				keyList.Add(new KeyValuePair<string, char>("0", numKey));

			}
		}

	}


	/// <summary>
	/// A Lookup method is one that maps some strings to double values.  Given a string,
	/// such a function can either return a double (meaning that the string maps to the
	/// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
	/// to a value. Exactly how a Lookup method decides which strings map to doubles and which
	/// don't is up to the implementation of the method.
	/// </summary>
	public delegate double Lookup(string var);



	/// <summary>
	/// converts formula to standard formula format
	/// </summary>
	/// <param name="var"></param>
	/// <returns></returns>
	public delegate string Normalizer(string var);

	/// <summary>
	/// validates the normailzed formula
	/// </summary>
	/// <param name="var"></param>
	/// <returns></returns>
	public delegate bool Validator(string var);
	/// <summary>
	/// Used to report that a Lookup delegate is unable to determine the value
	/// of a variable.
	/// </summary>
	[Serializable]
	public class UndefinedVariableException : Exception
	{
		/// <summary>
		/// Constructs an UndefinedVariableException containing whose message is the
		/// undefined variable.
		/// </summary>
		/// <param name="variable"></param>
		public UndefinedVariableException(String variable)
			: base(variable)
		{
		}
	}

	/// <summary>
	/// Used to report syntactic errors in the parameter to the Formula constructor.
	/// </summary>
	[Serializable]
	public class FormulaFormatException : Exception
	{
		/// <summary>
		/// Constructs a FormulaFormatException containing the explanatory message.
		/// </summary>
		public FormulaFormatException(String message) : base(message)
		{
		}
	}

	/// <summary>
	/// Used to report errors that occur when evaluating a Formula.
	/// </summary>
	[Serializable]
	public class FormulaEvaluationException : Exception
	{
		/// <summary>
		/// Constructs a FormulaEvaluationException containing the explanatory message.
		/// </summary>
		public FormulaEvaluationException(String message) : base(message)
		{
		}
	}
}
