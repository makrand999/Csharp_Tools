# Trivia Challenge

A real-time trivia game that fetches questions from the Open Trivia Database API.

## Usage

```bash
dotnet run --project Trivia.csproj
```

## Example

```
🎯 Trivia Challenge
===================

Select difficulty:
  1. Easy
  2. Medium
  3. Hard

Choice (1-3): 2

How many questions? (5-20, default=10): 10

Fetching 10 medium questions...

--------------------------------------------------
Category: Science & Nature
Question: What is the chemical symbol for Gold?

  1. Ag
  2. Au
  3. Fe
  4. Pb

Your answer (1-4, or 'q' to quit): 2
✅ Correct!
Score: 1 | Streak: 1
```

## Features

- **API integration** - Fetches real questions from OpenTDB
- **Difficulty levels** - Easy, Medium, Hard
- **Customizable length** - 5-20 questions
- **Streak tracking** - Bonus for consecutive correct answers
- **Categories** - Various trivia categories
- **Quit anytime** - Press 'q' to exit

## Concepts Demonstrated

- HTTP client with async/await
- JSON deserialization
- HTML entity decoding
- Random shuffling
- Score and streak tracking
