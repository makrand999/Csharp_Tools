using System.Globalization;

namespace MathExpressionParser;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Math Expression Parser - Evaluate mathematical expressions");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project MathExpressionParser.csproj -- <expression>");
            Console.WriteLine("  dotnet run --project MathExpressionParser.csproj -- interactive");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  2 + 3 * 4");
            Console.WriteLine("  (10 - 5) / 2");
            Console.WriteLine("  2 ^ 10");
            Console.WriteLine("  sin(PI / 2) + cos(0)");
            Console.WriteLine("  sqrt(16) + log(100)");
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            RunInteractiveMode();
            return 0;
        }

        var expression = string.Join(" ", args);
        var parser = new ExpressionParser();
        
        try
        {
            var result = parser.Evaluate(expression);
            Console.WriteLine($"{expression} = {result}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Math Expression Parser (Interactive Mode)");
        Console.WriteLine("Type 'quit' or 'exit' to exit.");
        Console.WriteLine("Type 'help' for available functions.");
        Console.WriteLine();

        var parser = new ExpressionParser();

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                ShowHelp();
                continue;
            }

            try
            {
                var result = parser.Evaluate(input);
                Console.WriteLine($"= {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Operators: +, -, *, /, ^ (power), % (modulo)");
        Console.WriteLine("Functions: sin, cos, tan, asin, acos, atan, sqrt, cbrt, log, log10, ln, exp, abs, floor, ceil, round, sign");
        Console.WriteLine("Constants: PI, E");
        Console.WriteLine("Examples:");
        Console.WriteLine("  2 + 3 * 4");
        Console.WriteLine("  sin(PI / 2)");
        Console.WriteLine("  sqrt(2) ^ 2");
        Console.WriteLine("  log10(1000)");
        Console.WriteLine();
    }
}

public enum TokenType
{
    Number,
    Operator,
    Function,
    LeftParen,
    RightParen,
    Comma,
    Constant,
    End
}

public record Token(TokenType Type, string Value, double? NumericValue = null);

public class ExpressionParser
{
    private readonly Dictionary<string, Func<double, double>> _functions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sin"] = Math.Sin,
        ["cos"] = Math.Cos,
        ["tan"] = Math.Tan,
        ["asin"] = Math.Asin,
        ["acos"] = Math.Acos,
        ["atan"] = Math.Atan,
        ["sqrt"] = Math.Sqrt,
        ["cbrt"] = Math.Cbrt,
        ["log"] = Math.Log10,
        ["log10"] = Math.Log10,
        ["ln"] = Math.Log,
        ["exp"] = Math.Exp,
        ["abs"] = Math.Abs,
        ["floor"] = Math.Floor,
        ["ceil"] = Math.Ceiling,
        ["round"] = Math.Round,
        ["sign"] = x => Math.Sign(x)
    };

    private readonly Dictionary<string, double> _constants = new(StringComparer.OrdinalIgnoreCase)
    {
        ["PI"] = Math.PI,
        ["E"] = Math.E
    };

    private readonly Dictionary<string, int> _precedence = new()
    {
        ["+"] = 1,
        ["-"] = 1,
        ["*"] = 2,
        ["/"] = 2,
        ["%"] = 2,
        ["^"] = 3
    };

    private readonly HashSet<string> _rightAssociative = ["^"];

    public double Evaluate(string expression)
    {
        var tokens = Tokenize(expression);
        var rpn = ConvertToRpn(tokens);
        return EvaluateRpn(rpn);
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

                var numberStr = expression.Substring(start, i - start);
                if (double.TryParse(numberStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    tokens.Add(new Token(TokenType.Number, numberStr, value));
                }
                else
                {
                    throw new FormatException($"Invalid number: {numberStr}");
                }
                continue;
            }

            if (char.IsLetter(c))
            {
                var start = i;
                while (i < expression.Length && char.IsLetter(expression[i]))
                    i++;

                var identifier = expression.Substring(start, i - start);
                
                if (_constants.TryGetValue(identifier, out var constantValue))
                {
                    tokens.Add(new Token(TokenType.Constant, identifier, constantValue));
                }
                else if (_functions.ContainsKey(identifier))
                {
                    tokens.Add(new Token(TokenType.Function, identifier));
                }
                else
                {
                    throw new InvalidOperationException($"Unknown identifier: {identifier}");
                }
                continue;
            }

            if ("+-*/%^".Contains(c))
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

            throw new InvalidOperationException($"Unexpected character: {c}");
        }

        tokens.Add(new Token(TokenType.End, ""));
        return tokens;
    }

    private List<Token> ConvertToRpn(List<Token> tokens)
    {
        var output = new List<Token>();
        var stack = new Stack<Token>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            switch (token.Type)
            {
                case TokenType.Number:
                case TokenType.Constant:
                    output.Add(token);
                    break;

                case TokenType.Function:
                    stack.Push(token);
                    break;

                case TokenType.Operator:
                    while (stack.Count > 0)
                    {
                        var top = stack.Peek();
                        if (top.Type == TokenType.Operator)
                        {
                            var precTop = _precedence[top.Value];
                            var precCurr = _precedence[token.Value];
                            
                            if ((_rightAssociative.Contains(token.Value) && precCurr < precTop) ||
                                (!_rightAssociative.Contains(token.Value) && precCurr <= precTop))
                            {
                                output.Add(stack.Pop());
                                continue;
                            }
                        }
                        else if (top.Type == TokenType.Function)
                        {
                            output.Add(stack.Pop());
                            continue;
                        }
                        break;
                    }
                    stack.Push(token);
                    break;

                case TokenType.LeftParen:
                    stack.Push(token);
                    break;

                case TokenType.RightParen:
                    while (stack.Count > 0 && stack.Peek().Type != TokenType.LeftParen)
                    {
                        output.Add(stack.Pop());
                    }
                    if (stack.Count == 0)
                        throw new InvalidOperationException("Mismatched parentheses");
                    
                    stack.Pop(); // Remove left paren
                    
                    if (stack.Count > 0 && stack.Peek().Type == TokenType.Function)
                    {
                        output.Add(stack.Pop());
                    }
                    break;

                case TokenType.Comma:
                    while (stack.Count > 0 && stack.Peek().Type != TokenType.LeftParen)
                    {
                        output.Add(stack.Pop());
                    }
                    break;

                case TokenType.End:
                    break;
            }
        }

        while (stack.Count > 0)
        {
            var token = stack.Pop();
            if (token.Type == TokenType.LeftParen)
                throw new InvalidOperationException("Mismatched parentheses");
            output.Add(token);
        }

        return output;
    }

    private double EvaluateRpn(List<Token> rpn)
    {
        var stack = new Stack<double>();

        foreach (var token in rpn)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                case TokenType.Constant:
                    stack.Push(token.NumericValue!.Value);
                    break;

                case TokenType.Operator:
                    if (stack.Count < 2)
                        throw new InvalidOperationException($"Operator {token.Value} requires 2 operands");
                    
                    var b = stack.Pop();
                    var a = stack.Pop();
                    
                    var result = token.Value switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => a / b,
                        "%" => a % b,
                        "^" => Math.Pow(a, b),
                        _ => throw new InvalidOperationException($"Unknown operator: {token.Value}")
                    };
                    stack.Push(result);
                    break;

                case TokenType.Function:
                    if (stack.Count < 1)
                        throw new InvalidOperationException($"Function {token.Value} requires 1 operand");
                    
                    var arg = stack.Pop();
                    
                    if (!_functions.TryGetValue(token.Value, out var func))
                        throw new InvalidOperationException($"Unknown function: {token.Value}");
                    
                    stack.Push(func(arg));
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected token: {token.Value}");
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Invalid expression");

        return stack.Pop();
    }
}
