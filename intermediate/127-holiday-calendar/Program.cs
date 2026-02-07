using System.Text.Json;

namespace HolidayCalendar;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Holiday Calendar Lookup (Nager.Date API - Free, no key required)");
            Console.WriteLine("Usage: dotnet run --project 127-holiday-calendar.csproj -- <country-code> [year]");
            Console.WriteLine("Example: dotnet run --project 127-holiday-calendar.csproj -- US 2026");
            Console.WriteLine("         dotnet run --project 127-holiday-calendar.csproj -- GB");
            Console.WriteLine("\nCountry codes: US, GB, DE, FR, JP, CN, AU, CA, BR, IN, etc.");
            return;
        }

        string countryCode = args[0].ToUpper();
        int year = DateTime.Now.Year;
        if (args.Length >= 2)
        {
            int.TryParse(args[1], out year);
        }

        var httpClient = new HttpClient();
        var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

        try
        {
            Console.WriteLine($"Fetching holidays for {countryCode} ({year})");
            Console.WriteLine();

            var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Error: Country code '{countryCode}' not found.");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var holidays = JsonSerializer.Deserialize<List<Holiday>>(json) ?? new List<Holiday>();

            Console.WriteLine($"=== {countryCode} Holidays ({year}) ===");
            Console.WriteLine();

            foreach (var holiday in holidays)
            {
                var dateStr = holiday.date.ToString("ddd, MMM dd");
                var types = holiday.types != null && holiday.types.Any() 
                    ? $" [{string.Join(", ", holiday.types)}]" 
                    : "";
                
                Console.WriteLine($"{dateStr} - {holiday.localName}");
                if (!string.IsNullOrEmpty(holiday.name) && holiday.name != holiday.localName)
                {
                    Console.WriteLine($"                 {holiday.name}{types}");
                }
                else
                {
                    Console.WriteLine($"                 {types}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Total holidays: {holidays.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

class Holiday
{
    public DateTime date { get; set; }
    public string? localName { get; set; }
    public string? name { get; set; }
    public string? countryCode { get; set; }
    public bool fixedDate { get; set; }
    public bool isPublicHoliday { get; set; }
    public int? holidayId { get; set; }
    public List<string>? types { get; set; }
}
