using System.Text.Json;

namespace CurrencyConverter;

class Program
{
    private static readonly Dictionary<string, string> CurrencyNames = new()
    {
        { "USD", "United States Dollar" },
        { "EUR", "Euro" },
        { "GBP", "British Pound Sterling" },
        { "JPY", "Japanese Yen" },
        { "CNY", "Chinese Yuan" },
        { "AUD", "Australian Dollar" },
        { "CAD", "Canadian Dollar" },
        { "CHF", "Swiss Franc" },
        { "INR", "Indian Rupee" },
        { "BRL", "Brazilian Real" },
        { "RUB", "Russian Ruble" },
        { "KRW", "South Korean Won" },
        { "MXN", "Mexican Peso" },
        { "ZAR", "South African Rand" },
        { "SGD", "Singapore Dollar" },
        { "HKD", "Hong Kong Dollar" },
        { "NOK", "Norwegian Krone" },
        { "SEK", "Swedish Krona" },
        { "DKK", "Danish Krone" },
        { "NZD", "New Zealand Dollar" },
        { "TRY", "Turkish Lira" },
        { "PLN", "Polish Zloty" },
        { "THB", "Thai Baht" },
        { "IDR", "Indonesian Rupiah" },
        { "MYR", "Malaysian Ringgit" },
        { "PHP", "Philippine Peso" },
        { "CZK", "Czech Koruna" },
        { "ILS", "Israeli New Shekel" },
        { "AED", "United Arab Emirates Dirham" },
        { "SAR", "Saudi Riyal" }
    };

    static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Currency Converter (using free exchange rate API)");
            Console.WriteLine("Usage: dotnet run --project 126-currency-converter.csproj -- <amount> <from> <to>");
            Console.WriteLine("Example: dotnet run --project 126-currency-converter.csproj -- 100 USD EUR");
            Console.WriteLine("         dotnet run --project 126-currency-converter.csproj -- 1000 JPY USD");
            Console.WriteLine("\nPopular currencies: USD, EUR, GBP, JPY, CNY, AUD, CAD, CHF, INR, BRL");
            return;
        }

        if (!decimal.TryParse(args[0], out decimal amount))
        {
            Console.WriteLine("Error: Invalid amount");
            return;
        }

        string fromCurrency = args[1].ToUpper();
        string toCurrency = args[2].ToUpper();

        var httpClient = new HttpClient();
        var url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}";

        try
        {
            Console.WriteLine($"Converting {amount:N2} {fromCurrency} to {toCurrency}");
            Console.WriteLine();

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("rates", out var rates) || !rates.TryGetProperty(toCurrency, out var rateProp))
            {
                Console.WriteLine($"Error: Could not find exchange rate for {toCurrency}");
                return;
            }

            var rate = rateProp.GetDouble();
            var convertedAmount = amount * (decimal)rate;
            var timestamp = GetString(root, "date");

            Console.WriteLine("=== Conversion Result ===");
            Console.WriteLine($"{amount:N2} {fromCurrency} = {convertedAmount:N2} {toCurrency}");
            Console.WriteLine($"Exchange Rate: 1 {fromCurrency} = {rate:F6} {toCurrency}");
            Console.WriteLine($"1 {toCurrency} = {1.0 / rate:F6} {fromCurrency}");
            Console.WriteLine($"Rate Date: {timestamp}");
            Console.WriteLine();

            // Show info about currencies
            if (CurrencyNames.TryGetValue(fromCurrency, out var fromName))
            {
                Console.WriteLine($"{fromCurrency}: {fromName}");
            }
            if (CurrencyNames.TryGetValue(toCurrency, out var toName))
            {
                Console.WriteLine($"{toCurrency}: {toName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
            ? prop.GetString() ?? "N/A"
            : "N/A";
    }
}
