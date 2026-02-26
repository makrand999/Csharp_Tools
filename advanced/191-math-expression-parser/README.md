# Math Expression Parser

A powerful mathematical expression evaluator that parses and calculates complex mathematical expressions with support for operators, functions, and constants.

## Usage

```bash
# Single expression
dotnet run --project MathExpressionParser.csproj -- "2 + 3 * 4"

# Interactive mode
dotnet run --project MathExpressionParser.csproj -- interactive
```

## Examples

```
$ dotnet run --project MathExpressionParser.csproj -- "2 + 3 * 4"
2 + 3 * 4 = 14

$ dotnet run --project MathExpressionParser.csproj -- "(10 - 5) / 2"
(10 - 5) / 2 = 2.5

$ dotnet run --project MathExpressionParser.csproj -- "2 ^ 10"
2 ^ 10 = 1024

$ dotnet run --project MathExpressionParser.csproj -- "sin(PI / 2) + cos(0)"
sin(PI / 2) + cos(0) = 2

$ dotnet run --project MathExpressionParser.csproj -- "sqrt(16) + log(100)"
sqrt(16) + log(100) = 6
```

## Supported Features

### Operators
- `+` Addition
- `-` Subtraction
- `*` Multiplication
- `/` Division
- `%` Modulo
- `^` Power (exponentiation)

### Functions
- Trigonometric: `sin`, `cos`, `tan`, `asin`, `acos`, `atan`
- Roots: `sqrt`, `cbrt`
- Logarithmic: `log`, `log10`, `ln`, `exp`
- Utility: `abs`, `floor`, `ceil`, `round`, `sign`

### Constants
- `PI` - π (3.14159...)
- `E` - Euler's number (2.71828...)

## Concepts Demonstrated

- Tokenization (lexical analysis)
- Shunting-yard algorithm for RPN conversion
- Reverse Polish Notation (RPN) evaluation
- Operator precedence and associativity
- Stack-based expression evaluation
- Recursive descent parsing concepts
