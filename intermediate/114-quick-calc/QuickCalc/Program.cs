using System.Text.Json;

var dataFile = "calc_history.json";
var variables = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
var history = LoadHistory(dataFile);
var maxHistory = 100;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "eval":
        Evaluate(args.Skip(1).ToArray());
        break;
    case "var":
        SetVariable(args.Skip(1).ToArray());
        break;
    case "vars":
        ListVariables();
        break;
    case "history":
        ShowHistory();
        break;
    case "clear":
        ClearHistory();
        break;
    case "save":
        SaveVariable(args.Skip(1).ToArray());
        break;
    default:
        Evaluate(args);
        break;
}

void Evaluate(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- eval <expression>");
        Console.WriteLine("   or: dotnet run -- <expression>");
        return;
    }

    var expression = string.Join(" ", args);

    try
    {
        var result = EvaluateExpression(expression);
        Console.WriteLine($"{expression} = {result}");

        history.Insert(0, new CalcEntry
        {
            Expression = expression,
            Result = result,
            EvaluatedAt = DateTime.Now
        });

        if (history.Count > maxHistory)
            history.RemoveAt(history.Count - 1);

        SaveHistory(dataFile);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void SetVariable(string[] args)
{
    if (args.Length < 2 || !args.Contains("="))
    {
        Console.WriteLine("Usage: dotnet run -- var <name> = <value>");
        Console.WriteLine("   or: dotnet run -- var <name> = <expression>");
        return;
    }

    var eqIndex = Array.IndexOf(args, "=");
    if (eqIndex < 1 || eqIndex >= args.Length - 1)
    {
        Console.WriteLine("Invalid variable assignment");
        return;
    }

    var name = args[0];
    var expr = string.Join(" ", args.Skip(2));

    try
    {
        var value = EvaluateExpression(expr);
        variables[name] = value;
        Console.WriteLine($"{name} = {value}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ListVariables()
{
    if (variables.Count == 0)
    {
        Console.WriteLine("No variables defined");
        return;
    }

    Console.WriteLine("\nVariables:");
    foreach (var kvp in variables)
    {
        Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
    }
    Console.WriteLine();
}

void ShowHistory()
{
    if (history.Count == 0)
    {
        Console.WriteLine("History is empty");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"{"ID",-4} {"Expression",-30} Result");
    Console.WriteLine(new string('-', 50));

    var items = history.Take(20).ToList();
    foreach (var item in items)
    {
        var expr = item.Expression.Length > 27
            ? item.Expression[..24] + "..."
            : item.Expression;
        Console.WriteLine($"{item.Id,-4} {expr,-30} {item.Result}");
    }
    Console.WriteLine($"\nTotal: {history.Count} item(s)");
}

void ClearHistory()
{
    history.Clear();
    SaveHistory(dataFile);
    Console.WriteLine("✓ History cleared");
}

void SaveVariable(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- save <name> <expression>");
        return;
    }

    var name = args[0];
    var expr = string.Join(" ", args.Skip(1));

    try
    {
        var value = EvaluateExpression(expr);
        variables[name] = value;
        Console.WriteLine($"✓ Saved {name} = {value}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        QuickCalc - Expression evaluator with history and variables

        Usage:
          dotnet run -- [eval] <expression>
          dotnet run -- var <name> = <expression>
          dotnet run -- vars
          dotnet run -- history
          dotnet run -- clear

        Features:
          - Basic arithmetic: +, -, *, /, %
          - Powers: ^ or **
          - Functions: sin, cos, tan, sqrt, log, ln, exp, abs, round
          - Constants: pi, e
          - Variables: store and reuse values
          - History: automatic history tracking

        Examples:
          dotnet run -- 2 + 2 * 3
          dotnet run -- eval sin(pi / 2)
          dotnet run -- var x = 10
          dotnet run -- var y = x * 2 + 5
          dotnet run -- sqrt(16) + log(100)
          dotnet run -- 2 ^ 10
          dotnet run -- round(pi * 100) / 100
        """);
}

double EvaluateExpression(string expr)
{
    if (string.IsNullOrWhiteSpace(expr))
        throw new ArgumentException("Empty expression");

    var working = expr.ToLower().Trim();

    // Replace constants
    working = working.Replace("pi", Math.PI.ToString());
    working = working.Replace("e", Math.E.ToString());

    // Replace variables
    foreach (var kvp in variables)
    {
        working = System.Text.RegularExpressions.Regex.Replace(
            working, $@"\b{kvp.Key}\b", kvp.Value.ToString());
    }

    // Replace operators
    working = working.Replace("**", "^");

    // Replace functions
    working = working.Replace("sin", "Math.Sin");
    working = working.Replace("cos", "Math.Cos");
    working = working.Replace("tan", "Math.Tan");
    working = working.Replace("sqrt", "Math.Sqrt");
    working = working.Replace("log", "Math.Log10");
    working = working.Replace("ln", "Math.Log");
    working = working.Replace("exp", "Math.Exp");
    working = working.Replace("abs", "Math.Abs");
    working = working.Replace("round", "Math.Round");
    working = working.Replace("floor", "Math.Floor");
    working = working.Replace("ceil", "Math.Ceiling");

    try
    {
        var result = new System.Data.DataTable().Compute(working, null);
        return Convert.ToDouble(result);
    }
    catch
    {
        throw new ArgumentException($"Invalid expression: {expr}");
    }
}

static List<CalcEntry> LoadHistory(string path)
{
    if (!File.Exists(path))
        return [];

    try
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<CalcEntry>>(json) ?? [];
    }
    catch
    {
        return new List<CalcEntry>();
    }
}

void SaveHistory(string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(history, options);
    File.WriteAllText(path, json);
}

class CalcEntry
{
    public int Id { get; set; }
    public required string Expression { get; set; }
    public double Result { get; set; }
    public DateTime EvaluatedAt { get; set; }
}
