using System.Text.Json;

namespace RecipeManager;

class Program
{
    private const string DataFile = "recipes.json";
    private static List<Recipe> _recipes = new();

    static void Main(string[] args)
    {
        Console.WriteLine("🍳 Recipe Manager");
        Console.WriteLine("=================\n");

        LoadRecipes();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "add": AddRecipe(); break;
            case "list": ListRecipes(); break;
            case "search": SearchRecipes(args.Length > 1 ? string.Join(" ", args.Skip(1)) : null); break;
            case "get": GetRecipe(args.ElementAtOrDefault(1)); break;
            case "delete": DeleteRecipe(args.ElementAtOrDefault(1)); break;
            default: ShowMenu(); break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add     - Add new recipe");
        Console.WriteLine("  list    - List all recipes");
        Console.WriteLine("  search  - Search recipes");
        Console.WriteLine("  get     - View recipe details");
        Console.WriteLine("  delete  - Delete recipe");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 2);
        string cmd = parts[0].ToLower();
        string? arg = parts.Length > 1 ? parts[1] : null;

        switch (cmd)
        {
            case "add": AddRecipe(); break;
            case "list": ListRecipes(); break;
            case "search": SearchRecipes(arg); break;
            case "get": GetRecipe(arg); break;
            case "delete": DeleteRecipe(arg); break;
        }
    }

    private static void LoadRecipes()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _recipes = JsonSerializer.Deserialize<List<Recipe>>(json) ?? new List<Recipe>();
        }
    }

    private static void SaveRecipes()
    {
        string json = JsonSerializer.Serialize(_recipes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void AddRecipe()
    {
        Console.Write("\nRecipe Name: ");
        string? name = Console.ReadLine();

        Console.Write("Category (e.g., Breakfast, Dinner, Dessert): ");
        string? category = Console.ReadLine();

        Console.Write("Servings: ");
        string? servingsStr = Console.ReadLine();
        int.TryParse(servingsStr, out int servings);

        Console.Write("Prep Time (minutes): ");
        string? prepStr = Console.ReadLine();
        int.TryParse(prepStr, out int prepTime);

        Console.Write("Cook Time (minutes): ");
        string? cookStr = Console.ReadLine();
        int.TryParse(cookStr, out int cookTime);

        Console.WriteLine("\nIngredients (one per line, format: 'amount unit ingredient', empty line to finish):");
        var ingredients = new List<string>();
        string? line;
        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            ingredients.Add(line.Trim());
        }

        Console.WriteLine("\nInstructions (one step per line, empty line to finish):");
        var instructions = new List<string>();
        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            instructions.Add(line.Trim());
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Recipe name is required!");
            return;
        }

        var recipe = new Recipe
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = name!,
            Category = category ?? "Uncategorized",
            Servings = servings > 0 ? servings : 4,
            PrepTime = prepTime,
            CookTime = cookTime,
            Ingredients = ingredients,
            Instructions = instructions,
            CreatedAt = DateTime.Now
        };

        _recipes.Add(recipe);
        SaveRecipes();

        Console.WriteLine($"\n✅ Recipe added: {recipe.Name}");
        Console.WriteLine($"   ID: {recipe.Id}");
        Console.WriteLine($"   Total Time: {recipe.TotalTime} minutes");
    }

    private static void ListRecipes()
    {
        if (_recipes.Count == 0)
        {
            Console.WriteLine("No recipes found.");
            return;
        }

        Console.Write("\nFilter by category (or Enter for all): ");
        string? category = Console.ReadLine();

        var filtered = string.IsNullOrWhiteSpace(category) 
            ? _recipes 
            : _recipes.Where(r => r.Category.Equals(category!.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

        if (filtered.Count == 0)
        {
            Console.WriteLine("No recipes found in that category.");
            return;
        }

        Console.WriteLine($"\n{"ID",-10} {"Name",-30} {"Category",-15} {"Time",-10} {"Servings",-10}");
        Console.WriteLine(new string('-', 80));

        foreach (var recipe in filtered.OrderBy(r => r.Name))
        {
            Console.WriteLine($"{recipe.Id,-10} {Truncate(recipe.Name, 30),-30} {recipe.Category,-15} {recipe.TotalTime,-10} {recipe.Servings,-10}");
        }

        Console.WriteLine($"\nTotal: {filtered.Count} recipe(s)");
    }

    private static void SearchRecipes(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Console.Write("Search query: ");
            query = Console.ReadLine();
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("No search query provided.");
            return;
        }

        var results = _recipes.Where(r =>
            r.Name.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            r.Category.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            r.Ingredients.Any(i => i.Contains(query!, StringComparison.OrdinalIgnoreCase)) ||
            r.Instructions.Any(i => i.Contains(query!, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        if (results.Count == 0)
        {
            Console.WriteLine($"No recipes found matching '{query}'.");
            return;
        }

        Console.WriteLine($"\nFound {results.Count} recipe(s):\n");
        foreach (var recipe in results)
        {
            Console.WriteLine($"[{recipe.Id}] {recipe.Name}");
            Console.WriteLine($"  Category: {recipe.Category} | Servings: {recipe.Servings} | Time: {recipe.TotalTime} min");
            Console.WriteLine();
        }
    }

    private static void GetRecipe(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Recipe ID: ");
            id = Console.ReadLine();
        }

        var recipe = _recipes.FirstOrDefault(r => r.Id == id);
        if (recipe == null)
        {
            Console.WriteLine($"Recipe '{id}' not found.");
            return;
        }

        Console.WriteLine($"\n🍳 {recipe.Name}");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Category: {recipe.Category}");
        Console.WriteLine($"Servings: {recipe.Servings}");
        Console.WriteLine($"Prep Time: {recipe.PrepTime} min | Cook Time: {recipe.CookTime} min | Total: {recipe.TotalTime} min");
        
        Console.WriteLine($"\n📝 Ingredients:");
        foreach (var ingredient in recipe.Ingredients)
        {
            Console.WriteLine($"  • {ingredient}");
        }

        Console.WriteLine($"\n📋 Instructions:");
        int step = 1;
        foreach (var instruction in recipe.Instructions)
        {
            Console.WriteLine($"  {step}. {instruction}");
            step++;
        }

        Console.WriteLine();
    }

    private static void DeleteRecipe(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Recipe ID to delete: ");
            id = Console.ReadLine();
        }

        var recipe = _recipes.FirstOrDefault(r => r.Id == id);
        if (recipe == null)
        {
            Console.WriteLine($"Recipe '{id}' not found.");
            return;
        }

        Console.Write($"Delete '{recipe.Name}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _recipes.Remove(recipe);
            SaveRecipes();
            Console.WriteLine("✅ Recipe deleted.");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}

class Recipe
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public int Servings { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public int TotalTime => PrepTime + CookTime;
}
