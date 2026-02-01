using System.Text.Json;
using System.Text.Json.Serialization;

var recipesFile = "recipes.json";
var recipes = LoadRecipes(recipesFile);

while (true)
{
    Console.WriteLine("\n=== Recipe Manager ===");
    Console.WriteLine("1. Add Recipe");
    Console.WriteLine("2. List Recipes");
    Console.WriteLine("3. View Recipe Details");
    Console.WriteLine("4. Search Recipes");
    Console.WriteLine("5. Delete Recipe");
    Console.WriteLine("6. Save/Export");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1": AddRecipe(recipes); break;
        case "2": ListRecipes(recipes); break;
        case "3": ViewRecipeDetails(recipes); break;
        case "4": SearchRecipes(recipes); break;
        case "5": DeleteRecipe(recipes); break;
        case "6": SaveRecipes(recipes, recipesFile); break;
        case "0": SaveAndExit(recipes, recipesFile); return;
        default: Console.WriteLine("Invalid option."); break;
    }
}

static List<Recipe> LoadRecipes(string path)
{
    if (!File.Exists(path)) return [];
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<List<Recipe>>(json) ?? [];
}

static void SaveRecipes(List<Recipe> recipes, string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(path, JsonSerializer.Serialize(recipes, options));
    Console.WriteLine($"Recipes saved to {path}");
}

static void AddRecipe(List<Recipe> recipes)
{
    Console.Write("Recipe name: ");
    var name = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Category (e.g., Breakfast, Dinner, Dessert): ");
    var category = Console.ReadLine() ?? "Uncategorized";

    Console.Write("Servings: ");
    if (!int.TryParse(Console.ReadLine(), out var servings) || servings <= 0)
    {
        Console.WriteLine("Invalid servings number.");
        return;
    }

    Console.Write("Prep time (minutes): ");
    if (!int.TryParse(Console.ReadLine(), out var prepTime) || prepTime < 0)
    {
        Console.WriteLine("Invalid prep time.");
        return;
    }

    Console.WriteLine("Enter ingredients (one per line, empty line to finish):");
    var ingredients = new List<Ingredient>();
    while (true)
    {
        Console.Write("  Ingredient: ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) break;

        var parts = line.Split('|');
        var ingredient = new Ingredient { Name = parts[0].Trim() };
        if (parts.Length > 1) ingredient.Amount = parts[1].Trim();
        if (parts.Length > 2) ingredient.Unit = parts[2].Trim();

        ingredients.Add(ingredient);
    }

    if (ingredients.Count == 0)
    {
        Console.WriteLine("At least one ingredient is required.");
        return;
    }

    Console.WriteLine("Enter instructions (one step per line, empty line to finish):");
    var instructions = new List<string>();
    var stepNum = 1;
    while (true)
    {
        Console.Write($"  Step {stepNum}: ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) break;
        instructions.Add(line.Trim());
        stepNum++;
    }

    if (instructions.Count == 0)
    {
        Console.WriteLine("At least one instruction is required.");
        return;
    }

    recipes.Add(new Recipe
    {
        Id = recipes.Count > 0 ? recipes.Max(r => r.Id) + 1 : 1,
        Name = name,
        Category = category,
        Servings = servings,
        PrepTimeMinutes = prepTime,
        Ingredients = ingredients,
        Instructions = instructions,
        CreatedAt = DateTime.Now
    });

    Console.WriteLine($"Recipe '{name}' added successfully!");
}

static void ListRecipes(List<Recipe> recipes)
{
    if (recipes.Count == 0)
    {
        Console.WriteLine("No recipes found.");
        return;
    }

    Console.WriteLine($"\n{"Id",-5} {"Name",-25} {"Category",-15} {"Servings",-10} {"Prep Time"}");
    Console.WriteLine(new string('-', 75));
    foreach (var recipe in recipes)
    {
        Console.WriteLine($"{recipe.Id,-5} {recipe.Name,-25} {recipe.Category,-15} {recipe.Servings,-10} {recipe.PrepTimeMinutes} min");
    }
}

static void ViewRecipeDetails(List<Recipe> recipes)
{
    Console.Write("Enter recipe ID: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var recipe = recipes.FirstOrDefault(r => r.Id == id);
    if (recipe == null)
    {
        Console.WriteLine($"Recipe with ID {id} not found.");
        return;
    }

    Console.WriteLine($"\n=== {recipe.Name} ===");
    Console.WriteLine($"Category: {recipe.Category}");
    Console.WriteLine($"Servings: {recipe.Servings}");
    Console.WriteLine($"Prep Time: {recipe.PrepTimeMinutes} minutes");
    Console.WriteLine($"\nIngredients:");
    foreach (var ing in recipe.Ingredients)
    {
        var amountStr = string.IsNullOrWhiteSpace(ing.Amount) ? "" : $"{ing.Amount} ";
        var unitStr = string.IsNullOrWhiteSpace(ing.Unit) ? "" : $" {ing.Unit}";
        Console.WriteLine($"  - {amountStr}{ing.Name}{unitStr}");
    }

    Console.WriteLine($"\nInstructions:");
    for (var i = 0; i < recipe.Instructions.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {recipe.Instructions[i]}");
    }
}

static void SearchRecipes(List<Recipe> recipes)
{
    Console.Write("Search query: ");
    var query = (Console.ReadLine() ?? "").ToLower();
    if (string.IsNullOrWhiteSpace(query))
    {
        Console.WriteLine("Query cannot be empty.");
        return;
    }

    var results = recipes.Where(r =>
        r.Name.ToLower().Contains(query) ||
        r.Category.ToLower().Contains(query) ||
        r.Ingredients.Any(i => i.Name.ToLower().Contains(query))
    ).ToList();

    if (results.Count == 0)
    {
        Console.WriteLine("No recipes found matching your search.");
        return;
    }

    Console.WriteLine($"\nFound {results.Count} recipe(s):");
    foreach (var recipe in results)
    {
        Console.WriteLine($"  {recipe.Name} - {recipe.Category} ({recipe.PrepTimeMinutes} min)");
    }
}

static void DeleteRecipe(List<Recipe> recipes)
{
    Console.Write("Enter recipe ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var recipe = recipes.FirstOrDefault(r => r.Id == id);
    if (recipe == null)
    {
        Console.WriteLine($"Recipe with ID {id} not found.");
        return;
    }

    Console.Write($"Are you sure you want to delete '{recipe.Name}'? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y")
    {
        Console.WriteLine("Deletion cancelled.");
        return;
    }

    recipes.Remove(recipe);
    Console.WriteLine($"Recipe '{recipe.Name}' deleted successfully!");
}

static void SaveAndExit(List<Recipe> recipes, string path)
{
    SaveRecipes(recipes, path);
    Console.WriteLine("Goodbye!");
}

class Recipe
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("servings")]
    public int Servings { get; set; }

    [JsonPropertyName("prepTimeMinutes")]
    public int PrepTimeMinutes { get; set; }

    [JsonPropertyName("ingredients")]
    public List<Ingredient> Ingredients { get; set; } = [];

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = [];

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

class Ingredient
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = "";

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "";
}
