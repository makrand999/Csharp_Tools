# Currency Converter

Convert between currencies using real-time exchange rates.

## Usage

```bash
dotnet run --project 126-currency-converter.csproj -- <amount> <from> <to>
```

## Example

```bash
dotnet run --project 126-currency-converter.csproj -- 100 USD EUR
dotnet run --project 126-currency-converter.csproj -- 1000 JPY USD
dotnet run --project 126-currency-converter.csproj -- 50 GBP JPY
```

### Sample Output

```
Converting 100.00 USD to EUR

=== Conversion Result ===
100.00 USD = 92.50 EUR
Exchange Rate: 1 USD = 0.925000 EUR
1 EUR = 1.081081 USD
Rate Date: 2026-04-01

USD: United States Dollar
EUR: Euro
```

## Supported Currencies

USD, EUR, GBP, JPY, CNY, AUD, CAD, CHF, INR, BRL, RUB, KRW, MXN, ZAR, SGD, HKD, NOK, SEK, DKK, NZD, TRY, PLN, THB, IDR, MYR, PHP, CZK, ILS, AED, SAR

## Concepts Demonstrated

- Exchange rate API integration
- Decimal arithmetic
- Currency code validation
- Reverse rate calculation
- Number formatting with culture
