# Recipe Manager

A CLI application for storing and managing recipes with ingredients, servings, and step-by-step instructions.

## Usage

```bash
dotnet run --project RecipeManager/RecipeManager.csproj
```

## Features

- **Add recipes** with name, category, servings, and prep time
- **Manage ingredients** with amount and unit
- **Step-by-step instructions** for cooking
- **Search recipes** by name, category, or ingredient
- **View detailed recipe** information
- **Delete recipes** with confirmation
- **Auto-save** to JSON file

## Example

```
=== Recipe Manager ===
1. Add Recipe
2. List Recipes
3. View Recipe Details
4. Search Recipes
5. Delete Recipe
6. Save/Export
0. Exit
Choose option: 1

Recipe name: Pancakes
Category: Breakfast
Servings: 4
Prep time (minutes): 20
Enter ingredients (one per line, empty line to finish):
  Ingredient: Flour|2|cups
  Ingredient: Eggs|2|
  Ingredient: Milk|1.5|cups
  
Enter instructions (one step per line, empty line to finish):
  Step 1: Mix dry ingredients
  Step 2: Add wet ingredients and stir
  Step 3: Cook on griddle until golden
Recipe 'Pancakes' added successfully!
```

## Concepts Demonstrated

- JSON serialization with nested objects
- File I/O for data persistence
- LINQ for searching and filtering
- Interactive CLI with multi-step input
- Collections and list operations
- Object composition (Recipe contains Ingredients)
