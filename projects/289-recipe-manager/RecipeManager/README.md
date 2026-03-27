# Recipe Manager

Store, organize, and search your favorite recipes with ingredients and instructions.

## Usage

```bash
# Interactive mode
dotnet run --project RecipeManager.csproj

# Add recipe
dotnet run --project RecipeManager.csproj add

# List recipes
dotnet run --project RecipeManager.csproj list

# Search recipes
dotnet run --project RecipeManager.csproj search "chicken"

# View recipe details
dotnet run --project RecipeManager.csproj get abc12345

# Delete recipe
dotnet run --project RecipeManager.csproj delete abc12345
```

## Example

```
🍳 Recipe Manager
=================

Commands:
  add     - Add new recipe
  list    - List all recipes
  search  - Search recipes
  get     - View recipe details
  delete  - Delete recipe

Enter command: add

Recipe Name: Spaghetti Carbonara
Category (e.g., Breakfast, Dinner, Dessert): Dinner
Servings: 4
Prep Time (minutes): 15
Cook Time (minutes): 20

Ingredients (one per line, format: 'amount unit ingredient'):
> 400g spaghetti
> 200g pancetta
> 4 egg yolks
> 100g parmesan
>
Instructions (one step per line):
> Boil pasta in salted water
> Fry pancetta until crispy
> Mix egg yolks with parmesan
> Combine all ingredients
>
✅ Recipe added: Spaghetti Carbonara
   ID: abc12345
   Total Time: 35 minutes
```

## Features

- **Recipe storage** - Name, category, servings, times
- **Ingredients list** - Structured ingredient entries
- **Step-by-step instructions** - Numbered cooking steps
- **Category filtering** - Organize by meal type
- **Search** - Find by name, ingredients, or instructions
- **Total time calculation** - Auto-calculated cook + prep time

## Data Storage

Recipes are stored in `recipes.json` in the current directory.

## Categories

Common categories include:
- Breakfast
- Lunch
- Dinner
- Dessert
- Snack
- Appetizer
- Beverage

## Concepts Demonstrated

- JSON serialization
- File I/O
- List management
- LINQ filtering and searching
- Data modeling with computed properties
