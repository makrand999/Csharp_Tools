using System.Text.Json;
using System.Text.Json.Serialization;

var budgetFile = "budget.json";
var budget = LoadBudget(budgetFile);

while (true)
{
    Console.WriteLine("\n=== Budget Planner ===");
    Console.WriteLine("1. Set Monthly Budget");
    Console.WriteLine("2. Add Expense");
    Console.WriteLine("3. View Budget Summary");
    Console.WriteLine("4. View Expenses by Category");
    Console.WriteLine("5. Delete Expense");
    Console.WriteLine("6. Save/Export");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1": SetMonthlyBudget(budget); break;
        case "2": AddExpense(budget); break;
        case "3": ViewBudgetSummary(budget); break;
        case "4": ViewExpensesByCategory(budget); break;
        case "5": DeleteExpense(budget); break;
        case "6": SaveBudget(budget, budgetFile); break;
        case "0": SaveAndExit(budget, budgetFile); return;
        default: Console.WriteLine("Invalid option."); break;
    }
}

static BudgetData LoadBudget(string path)
{
    if (!File.Exists(path)) return new BudgetData();
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<BudgetData>(json) ?? new BudgetData();
}

static void SaveBudget(BudgetData budget, string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(path, JsonSerializer.Serialize(budget, options));
    Console.WriteLine($"Budget saved to {path}");
}

static void SetMonthlyBudget(BudgetData budget)
{
    Console.Write("Enter monthly budget amount: $");
    if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
    {
        Console.WriteLine("Invalid amount. Please enter a positive number.");
        return;
    }

    budget.MonthlyBudget = amount;
    Console.WriteLine($"Monthly budget set to ${amount:F2}");
}

static void AddExpense(BudgetData budget)
{
    if (budget.MonthlyBudget == 0)
    {
        Console.WriteLine("Please set a monthly budget first (Option 1).");
        return;
    }

    Console.Write("Description: ");
    var description = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine("Description cannot be empty.");
        return;
    }

    Console.Write("Amount: $");
    if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    Console.Write("Category (e.g., Food, Transport, Entertainment, Bills, Other): ");
    var category = Console.ReadLine()?.Trim() ?? "Other";

    Console.Write("Date (YYYY-MM-DD, or Enter for today): ");
    var dateInput = Console.ReadLine();
    DateTime expenseDate;
    if (string.IsNullOrWhiteSpace(dateInput))
    {
        expenseDate = DateTime.Today;
    }
    else if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out expenseDate))
    {
        Console.WriteLine("Invalid date format. Using today's date.");
        expenseDate = DateTime.Today;
    }

    budget.Expenses.Add(new Expense
    {
        Id = budget.Expenses.Count > 0 ? budget.Expenses.Max(e => e.Id) + 1 : 1,
        Description = description,
        Amount = amount,
        Category = category,
        Date = expenseDate,
        AddedAt = DateTime.Now
    });

    Console.WriteLine($"Expense added: {description} - ${amount:F2} ({category})");
}

static void ViewBudgetSummary(BudgetData budget)
{
    if (budget.MonthlyBudget == 0)
    {
        Console.WriteLine("No monthly budget set. Please set one first (Option 1).");
        return;
    }

    if (budget.Expenses.Count == 0)
    {
        Console.WriteLine($"Monthly budget: ${budget.MonthlyBudget:F2}");
        Console.WriteLine("No expenses recorded yet.");
        return;
    }

    var totalExpenses = budget.Expenses.Sum(e => e.Amount);
    var remaining = budget.MonthlyBudget - totalExpenses;
    var percentage = (totalExpenses / budget.MonthlyBudget) * 100;

    Console.WriteLine("\n=== Budget Summary ===");
    Console.WriteLine($"Monthly budget:    ${budget.MonthlyBudget:F2}");
    Console.WriteLine($"Total expenses:    ${totalExpenses:F2}");
    Console.WriteLine($"Remaining:         ${remaining:F2}");
    Console.WriteLine($"Used:              {percentage:F1}%");

    if (remaining < 0)
    {
        Console.WriteLine("\n⚠️  WARNING: You've exceeded your budget!");
    }
    else if (remaining < budget.MonthlyBudget * 0.2m)
    {
        Console.WriteLine("\n⚡  Caution: Less than 20% of budget remaining.");
    }

    Console.WriteLine("\nExpenses by category:");
    var byCategory = budget.Expenses.GroupBy(e => e.Category)
                                    .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount), Count = g.Count() })
                                    .OrderByDescending(x => x.Total);

    Console.WriteLine($"{"Category",-15} {"Amount",-12} {"Count",-8} {"% of Budget"}");
    Console.WriteLine(new string('-', 50));
    foreach (var cat in byCategory)
    {
        var pct = (cat.Total / budget.MonthlyBudget) * 100;
        Console.WriteLine($"{cat.Category,-15} ${cat.Total,-11:F2} {cat.Count,-8} {pct,5:F1}%");
    }
}

static void ViewExpensesByCategory(BudgetData budget)
{
    if (budget.Expenses.Count == 0)
    {
        Console.WriteLine("No expenses recorded yet.");
        return;
    }

    Console.Write("Category (or Enter for all): ");
    var category = Console.ReadLine()?.Trim();

    var filtered = string.IsNullOrWhiteSpace(category)
        ? budget.Expenses.OrderByDescending(e => e.Date).Take(20).ToList()
        : budget.Expenses.Where(e => e.Category.ToLower() == category.ToLower())
                        .OrderByDescending(e => e.Date).ToList();

    if (filtered.Count == 0)
    {
        Console.WriteLine("No expenses found.");
        return;
    }

    Console.WriteLine($"\n{"Date",-12} {"Description",-25} {"Category",-15} {"Amount"}");
    Console.WriteLine(new string('-', 65));
    foreach (var e in filtered)
    {
        Console.WriteLine($"{e.Date:yyyy-MM-dd,-12} {e.Description,-25} {e.Category,-15} ${e.Amount:F2}");
    }

    var total = filtered.Sum(e => e.Amount);
    Console.WriteLine($"\nTotal: ${total:F2} ({filtered.Count} expenses)");
}

static void DeleteExpense(BudgetData budget)
{
    if (budget.Expenses.Count == 0)
    {
        Console.WriteLine("No expenses to delete.");
        return;
    }

    Console.Write("Enter expense ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var expense = budget.Expenses.FirstOrDefault(e => e.Id == id);
    if (expense == null)
    {
        Console.WriteLine($"Expense with ID {id} not found.");
        return;
    }

    Console.Write($"Delete '{expense.Description}' (${expense.Amount:F2})? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y")
    {
        Console.WriteLine("Deletion cancelled.");
        return;
    }

    budget.Expenses.Remove(expense);
    Console.WriteLine("Expense deleted successfully!");
}

static void SaveAndExit(BudgetData budget, string path)
{
    SaveBudget(budget, path);
    Console.WriteLine("Goodbye! Stay on budget!");
}

class BudgetData
{
    [JsonPropertyName("monthlyBudget")]
    public decimal MonthlyBudget { get; set; }

    [JsonPropertyName("expenses")]
    public List<Expense> Expenses { get; set; } = [];
}

class Expense
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("addedAt")]
    public DateTime AddedAt { get; set; }
}
