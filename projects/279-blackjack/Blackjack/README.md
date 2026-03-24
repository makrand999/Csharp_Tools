# Blackjack

Classic casino card game with betting and dealer AI.

## Usage

```bash
dotnet run --project Blackjack.csproj
```

## Example

```
🃏 Blackjack
============

💰 Your money: $100
Place your bet (0 to quit): $10

Dealer's hand:
  K♠ [?]

Your hand:
  7♥ A♣
  Total: 18

(H)it or (S)tand? s

Dealer's hand:
  K♠ 9♦
  Total: 19

Dealer total: 19
😞 Dealer wins!

💰 Your money: $90
----------------------------------------
```

## Rules

- Get closer to 21 than the dealer without going over
- Number cards = face value, Face cards = 10, Ace = 1 or 11
- Dealer must hit on 16 or less, stand on 17+
- Blackjack (21 with 2 cards) pays 3:2
- Starting money: $100

## Card Values

| Card | Value |
|------|-------|
| 2-10 | Face value |
| J, Q, K | 10 |
| A | 1 or 11 |

## Concepts Demonstrated

- Card deck creation and shuffling
- Hand value calculation with Ace handling
- Simple AI (dealer behavior)
- Money/betting system
- Game state management
