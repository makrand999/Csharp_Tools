# QuickCalc

Expression evaluator with history tracking and variable support. Perform calculations with basic arithmetic, math functions, constants, and user-defined variables.

## Usage

```bash
dotnet run --project QuickCalc.csproj -- [eval] <expression>
dotnet run --project QuickCalc.csproj -- var <name> = <expression>
dotnet run --project QuickCalc.csproj -- vars
dotnet run --project QuickCalc.csproj -- history
dotnet run --project QuickCalc.csproj -- clear
```

## Features

- **Basic arithmetic**: `+`, `-`, `*`, `/`, `%`
- **Powers**: `^` or `**`
- **Math functions**: `sin`, `cos`, `tan`, `sqrt`, `log`, `ln`, `exp`, `abs`, `round`, `floor`, `ceil`
- **Constants**: `pi`, `e`
- **Variables**: Store and reuse values
- **History**: Automatic tracking of all calculations

## Examples

```bash
# Basic calculations
dotnet run --project QuickCalc.csproj -- 2 + 2 * 3
dotnet run --project QuickCalc.csproj -- eval "10 / 3"

# Math functions
dotnet run --project QuickCalc.csproj -- sin(pi / 2)
dotnet run --project QuickCalc.csproj -- sqrt(16) + log(100)
dotnet run --project QuickCalc.csproj -- round(pi * 100) / 100

# Powers
dotnet run --project QuickCalc.csproj -- 2 ^ 10
dotnet run --project QuickCalc.csproj -- 5 ** 3

# Variables
dotnet run --project QuickCalc.csproj -- var x = 10
dotnet run --project QuickCalc.csproj -- var y = x * 2 + 5
dotnet run --project QuickCalc.csproj -- vars

# View history
dotnet run --project QuickCalc.csproj -- history
dotnet run --project QuickCalc.csproj -- clear
```

## Sample Output

```
2 + 2 * 3 = 8
sin(pi / 2) = 1
sqrt(16) + log(100) = 6

Variables:
  x = 10
  y = 25

ID   Expression                     Result
--------------------------------------------------
1    2 + 2 * 3                      8
2    sin(pi / 2)                    1
3    sqrt(16) + log(100)            6
```

## Data Storage

Calculations are stored in `calc_history.json` for session persistence.

## Concepts Demonstrated

- Expression evaluation using DataTable.Compute
- Variable storage and substitution
- JSON serialization for history persistence
- Math function integration
- Command-line argument parsing
- History tracking with limits
