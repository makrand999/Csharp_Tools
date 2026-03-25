# Decision Maker

A multi-purpose decision tool with yes/no answers, option selection, coin flip, and random numbers.

## Usage

```bash
# Interactive mode
dotnet run --project DecisionMaker.csproj

# Quick decision from arguments
dotnet run --project DecisionMaker.csproj "Option 1" "Option 2" "Option 3"
```

## Modes

### 1. Yes/No Question
Ask any yes/no question and get an oracle-style answer.

### 2. Choose from Options
Enter multiple options and let the tool decide.

### 3. Coin Flip
Flip a virtual coin for heads/tails decisions.

### 4. Random Number
Generate a random number within a range.

## Example

```
🔮 Decision Maker
=================

Modes:
  1. Yes/No question
  2. Choose from options
  3. Coin flip
  4. Random number

Choose mode (1-4): 1

Ask a yes/no question:
> Should I learn C#?

🔮 Consulting the oracle...

✅ Yes, definitely!
(Good question format!)
```

```
$ dotnet run --project DecisionMaker.csproj "Pizza" "Burgers" "Sushi"

🎯 Decision: Sushi
```

## Concepts Demonstrated

- Random number generation
- Interactive menus
- Array selection
- Animation with console output
- Command-line arguments
