# Poker Hand Evaluator

Evaluates 5-card poker hands and compares them.

## Usage

```bash
dotnet run --project Poker.csproj
```

## Modes

1. **Evaluate a hand** - Enter 5 cards to get the hand ranking
2. **Deal random hand** - Get a random 5-card hand with evaluation
3. **Compare two hands** - Enter two hands to see which wins

## Example

```
🃏 Poker Hand Evaluator
======================

Modes:
  1. Evaluate a hand
  2. Deal random hand
  3. Compare two hands

Choose mode (1-3): 2

Your hand:
  K♠ 7♥ 7♦ 3♣ 2♠

Pair
Pair of 7s
```

## Hand Rankings (Highest to Lowest)

| Rank | Hand | Description |
|------|------|-------------|
| 10 | Royal Flush | A-K-Q-J-10 same suit |
| 9 | Straight Flush | 5 consecutive same suit |
| 8 | Four of a Kind | 4 cards same rank |
| 7 | Full House | 3 of a kind + pair |
| 6 | Flush | 5 cards same suit |
| 5 | Straight | 5 consecutive ranks |
| 4 | Three of a Kind | 3 cards same rank |
| 3 | Two Pair | 2 different pairs |
| 2 | Pair | 2 cards same rank |
| 1 | High Card | None of the above |

## Card Input Format

- Rank: 2-9, T (10), J, Q, K, A
- Suit: S (Spades), H (Hearts), D (Diamonds), C (Clubs)
- Example: `AS` = Ace of Spades, `TH` = 10 of Hearts

## Concepts Demonstrated

- Poker hand evaluation algorithm
- Card deck management
- Hand ranking comparison
- Pattern matching for hand detection
- String formatting with Unicode suits
