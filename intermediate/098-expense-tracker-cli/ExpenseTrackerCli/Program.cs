using System.Text.Json;

var dataFile = "expenses.json";
var expenses = LoadExpenses(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
        AddExpense(expenses, args.Skip(1).ToArray());
        break;
    case "list":
        ListExpenses(expenses, args.Skip(1).ToArray());
        break;
    case "delete":
        DeleteExpense(expenses, args.Skip(1).ToArray());
        break;
    case "report":
        ShowReport(expenses, args.Skip(1).ToArray());
        break;
    case "summary":
        ShowSummary(expenses);
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

SaveExpenses(dataFile, expenses);

void ShowHelp()
{
    Console.WriteLine("""
        Expense Tracker CLI - Track expenses with categories and generate reports
        
        Usage:
          dotnet run --project ExpenseTrackerCli.csproj <command> [arguments]
        
        Commands:
          add <amount> <category> <description>    Add a new expense
          list [category]                          List all expenses or filter by category
          delete <id>                              Remove an expense
          report <month>                           Show monthly report (YYYY-MM format)
          summary                                  Show overall summary
        
        Categories: food, transport, utilities, entertainment, shopping, health, other
        
        Examples:
          dotnet run -- add 25.50 food "Lunch at restaurant"
          dotnet run -- list food
          dotnet run -- report 2026-03
          dotnet run -- summary
        """);
}

void AddExpense(List<Expense> expenses, string[] args)
{
    if (args.Length < 3 || !decimal.TryParse(args[0], out var amount))
    {
        Console.WriteLine("Usage: add <amount> <category> <description>");
        return;
    }

    var category = args[1].ToLower();
    var description = string.Join(" ", args.Skip(2));
    var validCategories = new[] { "food", "transport", "utilities", "entertainment", "shopping", "health", "other" };
    
    if (!validCategories.Contains(category))
    {
        Console.WriteLine($"Warning: '{category}' is not a standard category.");
        Console.WriteLine($"Standard categories: {string.Join(", ", validCategories)}");
    }

    var expense = new Expense
    {
        Id = expenses.Count > 0 ? expenses.Max(e => e.Id) + 1 : 1,
        Amount = amount,
        Category = category,
        Description = description,
        Date = DateTime.Now
    };

    expenses.Add(expense);
    Console.WriteLine($"✓ Added: ${amount:F2} - {category} - {description}");
    Console.WriteLine($"  ID: {expense.Id} | Date: {expense.Date:yyyy-MM-dd}");
}

void ListExpenses(List<Expense> expenses, string[] args)
{
    var filtered = expenses;
    
    if (args.Length > 0)
    {
        var category = args[0].ToLower();
        filtered = expenses.Where(e => e.Category == category).ToList();
    }

    if (filtered.Count == 0)
    {
        Console.WriteLine("No expenses found.");
        return;
    }

    // Group by month
    var grouped = filtered.GroupBy(e => e.Date.ToString("yyyy-MM"))
                          .OrderByDescending(g => g.Key);

    foreach (var monthGroup in grouped)
    {
        Console.WriteLine($"\n📅 {monthGroup.Key}");
        Console.WriteLine(new string('-', 50));
        
        foreach (var expense in monthGroup.OrderBy(e => e.Date))
        {
            Console.WriteLine($"[{expense.Id}] {expense.Date:MM/dd} | ${expense.Amount,6:F2} | {expense.Category,-12} | {expense.Description}");
        }

        Console.WriteLine($"     {"",6} ─────────────────");
        Console.WriteLine($"     Total: ${monthGroup.Sum(e => e.Amount),6:F2}");
    }
    
    Console.WriteLine($"\nTotal: {filtered.Count} expense(s), ${filtered.Sum(e => e.Amount):F2}");
}

void DeleteExpense(List<Expense> expenses, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: delete <id>");
        return;
    }

    var expense = expenses.FirstOrDefault(e => e.Id == id);
    if (expense == null)
    {
        Console.WriteLine($"Expense #{id} not found.");
        return;
    }

    expenses.Remove(expense);
    Console.WriteLine($"✓ Deleted: ${expense.Amount:F2} - {expense.Category} - {expense.Description}");
}

void ShowReport(List<Expense> expenses, string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: report <month> (YYYY-MM format)");
        Console.WriteLine("Example: report 2026-03");
        return;
    }

    var month = args[0];
    var monthExpenses = expenses.Where(e => e.Date.ToString("yyyy-MM") == month).ToList();

    if (monthExpenses.Count == 0)
    {
        Console.WriteLine($"No expenses found for {month}.");
        return;
    }

    Console.WriteLine($"""
        
        ═══════════════════════════════════════════
         Monthly Report: {month}
        ═══════════════════════════════════════════
        """);

    var byCategory = monthExpenses.GroupBy(e => e.Category)
                                   .OrderByDescending(g => g.Sum(e => e.Amount));

    Console.WriteLine("  Category        Amount    Count");
    Console.WriteLine("  ───────────────────────────────");

    foreach (var cat in byCategory)
    {
        var total = cat.Sum(e => e.Amount);
        var count = cat.Count();
        var percentage = (double)total / (double)monthExpenses.Sum(e => e.Amount) * 100;
        Console.WriteLine($"  {cat.Key,-14} ${total,7:F2}   {percentage,5:F1}%  ({count})");
    }
    
    Console.WriteLine("  ───────────────────────────────");
    Console.WriteLine($"  TOTAL          ${monthExpenses.Sum(e => e.Amount),7:F2}   100.0%  ({monthExpenses.Count})");
    
    // Daily average
    var daysInMonth = DateTime.DaysInMonth(int.Parse(month.Split('-')[0]), int.Parse(month.Split('-')[1]));
    var dailyAvg = monthExpenses.Sum(e => e.Amount) / daysInMonth;
    Console.WriteLine($"\n  Daily average: ${dailyAvg:F2}");
    Console.WriteLine();
}

void ShowSummary(List<Expense> expenses)
{
    if (expenses.Count == 0)
    {
        Console.WriteLine("No expenses tracked yet.");
        return;
    }

    var total = expenses.Sum(e => e.Amount);
    var firstExpense = expenses.Min(e => e.Date);
    var lastExpense = expenses.Max(e => e.Date);
    var days = (lastExpense - firstExpense).Days + 1;

    Console.WriteLine("""
        
        ═══════════════════════════════════════════
         Expense Summary
        ═══════════════════════════════════════════
        """);

    Console.WriteLine($"  Period:          {firstExpense:yyyy-MM-dd} to {lastExpense:yyyy-MM-dd} ({days} days)");
    Console.WriteLine($"  Total expenses:  {expenses.Count}");
    Console.WriteLine($"  Total spent:     ${total:F2}");
    Console.WriteLine($"  Daily average:   ${total / days:F2}");
    Console.WriteLine($"  Monthly average: ${total / days * 30:F2}");
    
    Console.WriteLine("\n  By Category:");
    Console.WriteLine("  ─────────────────────────────────────");
    
    var byCategory = expenses.GroupBy(e => e.Category)
                             .OrderByDescending(g => g.Sum(e => e.Amount));
    
    foreach (var cat in byCategory)
    {
        var catTotal = cat.Sum(e => e.Amount);
        var percentage = (double)catTotal / (double)total * 100;
        var bar = new string('█', (int)(percentage / 5));
        Console.WriteLine($"  {cat.Key,-14} ${catTotal,8:F2}  {percentage,5:F1}% {bar}");
    }
    
    Console.WriteLine();
}

List<Expense> LoadExpenses(string path)
{
    if (!File.Exists(path))
        return new List<Expense>();
    
    try
    {
        var json = File.ReadAllText(path);
        var expenses = JsonSerializer.Deserialize<List<Expense>>(json);
        return expenses ?? new List<Expense>();
    }
    catch (JsonException)
    {
        Console.WriteLine("Warning: Could not parse expenses.json, starting fresh.");
        return new List<Expense>();
    }
}

void SaveExpenses(string path, List<Expense> expenses)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(expenses, options);
    File.WriteAllText(path, json);
}

class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
}
