using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpressionEvaluator;

public enum TokenType
{
    Number,
    Operator,
    Function,
    LeftParen,
    RightParen,
    Comma
}

public record Token(TokenType Type, string Value);

public class ExpressionEvaluator
{
    private readonly Dictionary<string, Func<double[], double>> _functions = new();

    public ExpressionEvaluator()
    {
        _functions["sin"] = args => Math.Sin(args[0]);
        _functions["cos"] = args => Math.Cos(args[0]);
        _functions["tan"] = args => Math.Tan(args[0]);
        _functions["sqrt"] = args => Math.Sqrt(args[0]);
        _functions["log"] = args => Math.Log10(args[0]);
        _functions["ln"] = args => Math.Log(args[0]);
        _functions["abs"] = args => Math.Abs(args[0]);
        _functions["ceil"] = args => Math.Ceiling(args[0]);
        _functions["floor"] = args => Math.Floor(args[0]);
        _functions["round"] = args => Math.Round(args[0]);
        _functions["exp"] = args => Math.Exp(args[0]);
        _functions["max"] = args => args.Max();
        _functions["min"] = args => args.Min();
        _functions["avg"] = args => args.Average();
        _functions["sum"] = args => args.Sum();
        _functions["pow"] = args => Math.Pow(args[0], args[1]);
    }

    public double Evaluate(string expression)
    {
        var tokens = Tokenize(expression);
        var postfix = ToPostfix(tokens);
        return EvaluatePostfix(postfix);
    }

    private List<Token> Tokenize(string expression)
    {
        var tokens = new List<Token>();
        var i = 0;

        while (i < expression.Length)
        {
            var c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c) || c == '.')
            {
                var start = i;
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    i++;
                tokens.Add(new Token(TokenType.Number, expression.Substring(start, i - start)));
                continue;
            }

            if (char.IsLetter(c))
            {
                var start = i;
                while (i < expression.Length && char.IsLetter(expression[i]))
                    i++;
                tokens.Add(new Token(TokenType.Function, expression.Substring(start, i - start).ToLower()));
                continue;
            }

            if ("+-*/^".Contains(c))
            {
                tokens.Add(new Token(TokenType.Operator, c.ToString()));
                i++;
                continue;
            }

            if (c == '(')
            {
                tokens.Add(new Token(TokenType.LeftParen, "("));
                i++;
                continue;
            }

            if (c == ')')
            {
                tokens.Add(new Token(TokenType.RightParen, ")"));
                i++;
                continue;
            }

            if (c == ',')
            {
                tokens.Add(new Token(TokenType.Comma, ","));
                i++;
                continue;
            }

            throw new ArgumentException($"Unexpected character: {c}");
        }

        return tokens;
    }

    private List<Token> ToPostfix(List<Token> tokens)
    {
        var output = new List<Token>();
        var stack = new Stack<Token>();
        var precedence = new Dictionary<string, int> { ["+"] = 1, ["-"] = 1, ["*"] = 2, ["/"] = 2, ["^"] = 3 };

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    output.Add(token);
                    break;

                case TokenType.Function:
                    stack.Push(token);
                    break;

                case TokenType.Operator:
                    while (stack.Count > 0 &&
                           stack.Peek().Type == TokenType.Operator &&
                           precedence.GetValueOrDefault(stack.Peek().Value, 0) >= precedence.GetValueOrDefault(token.Value, 0))
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                    break;

                case TokenType.LeftParen:
                    stack.Push(token);
                    break;

                case TokenType.RightParen:
                    while (stack.Count > 0 && stack.Peek().Type != TokenType.LeftParen)
                        output.Add(stack.Pop());
                    if (stack.Count == 0) throw new ArgumentException("Mismatched parentheses");
                    stack.Pop();
                    if (stack.Count > 0 && stack.Peek().Type == TokenType.Function)
                        output.Add(stack.Pop());
                    break;

                case TokenType.Comma:
                    while (stack.Count > 0 && stack.Peek().Type != TokenType.LeftParen)
                        output.Add(stack.Pop());
                    break;
            }
        }

        while (stack.Count > 0)
        {
            var token = stack.Pop();
            if (token.Type == TokenType.LeftParen) throw new ArgumentException("Mismatched parentheses");
            output.Add(token);
        }

        return output;
    }

    private double EvaluatePostfix(List<Token> postfix)
    {
        var stack = new Stack<double>();

        foreach (var token in postfix)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    stack.Push(double.Parse(token.Value, CultureInfo.InvariantCulture));
                    break;

                case TokenType.Operator:
                    if (stack.Count < 2) throw new ArgumentException("Invalid expression");
                    var b = stack.Pop();
                    var a = stack.Pop();
                    var result = token.Value switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => a / b,
                        "^" => Math.Pow(a, b),
                        _ => throw new ArgumentException($"Unknown operator: {token.Value}")
                    };
                    stack.Push(result);
                    break;

                case TokenType.Function:
                    if (!_functions.ContainsKey(token.Value))
                        throw new ArgumentException($"Unknown function: {token.Value}");

                    var args = new List<double>();
                    while (stack.Count > 0 && stack.Peek() != double.NaN)
                    {
                        args.Insert(0, stack.Pop());
                        if (token.Value is "sin" or "cos" or "tan" or "sqrt" or "log" or "ln" or "abs" or "ceil" or "floor" or "round" or "exp")
                            break;
                        if (token.Value == "pow" && args.Count == 2)
                            break;
                    }

                    if (args.Count == 0) throw new ArgumentException($"Function {token.Value} requires arguments");
                    stack.Push(_functions[token.Value](args.ToArray()));
                    break;
            }
        }

        if (stack.Count != 1) throw new ArgumentException("Invalid expression");
        return stack.Pop();
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Expression Evaluator Demo ===\n");

        var evaluator = new ExpressionEvaluator();

        var expressions = new[]
        {
            "2 + 3",
            "10 - 4 * 2",
            "(2 + 3) * 4",
            "2 ^ 10",
            "100 / 4 + 25",
            "sqrt(144)",
            "sin(0)",
            "cos(3.14159)",
            "log(100)",
            "ln(2.71828)",
            "abs(-42)",
            "pow(2, 8)",
            "max(10, 20, 15, 30)",
            "min(5, 3, 8, 1)",
            "avg(10, 20, 30, 40)",
            "sum(1, 2, 3, 4, 5)",
            "2 + 3 * 4 - 5",
            "((2 + 3) * (4 - 1)) / 3",
            "sqrt(pow(3, 2) + pow(4, 2))",
            "sin(0) + cos(0) + abs(-1)",
        };

        Console.WriteLine("Evaluating expressions:\n");
        foreach (var expr in expressions)
        {
            try
            {
                var result = evaluator.Evaluate(expr);
                Console.WriteLine($"  {expr,-40} = {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {expr,-40} = ERROR: {ex.Message}");
            }
        }

        Console.WriteLine("\n--- Interactive Mode ---");
        Console.WriteLine("Enter expressions to evaluate (or 'quit' to exit):\n");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input) || input.ToLower() == "quit") break;

            try
            {
                var result = evaluator.Evaluate(input);
                Console.WriteLine($"  = {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
            }
        }
    }
}
