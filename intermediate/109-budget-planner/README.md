# Budget Planner

A CLI application for monthly budget planning and expense tracking with category-based analysis.

## Usage

```bash
dotnet run --project BudgetPlanner/BudgetPlanner.csproj
```

## Features

- **Set monthly budget** limit
- **Add expenses** with description, amount, category, and date
- **View budget summary** with remaining balance and warnings
- **Category breakdown** showing spending by category
- **Budget alerts** when approaching or exceeding limits
- **Delete expenses** with confirmation
- **Auto-save** to JSON file

## Example

```
=== Budget Planner ===
1. Set Monthly Budget
2. Add Expense
3. View Budget Summary
4. View Expenses by Category
5. Delete Expense
6. Save/Export
0. Exit
Choose option: 1

Enter monthly budget amount: $2000
Monthly budget set to $2000.00

Choose option: 2

Description: Grocery shopping
Amount: $150.50
Category: Food
Date (YYYY-MM-DD, or Enter for today): 
Expense added: Grocery shopping - $150.50 (Food)
```

## Concepts Demonstrated

- JSON serialization for data persistence
- Decimal arithmetic for financial calculations
- LINQ grouping and aggregation
- Percentage calculations
- Conditional warnings based on thresholds
- DateTime parsing and formatting
- Interactive CLI with validation
