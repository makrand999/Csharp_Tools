using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var client = new HttpClient();
var endpoint = "https://countries.trevorblades.com/graphql";

Console.WriteLine("GraphQL Query Tool");
Console.WriteLine("==================");
Console.WriteLine($"Default endpoint: {endpoint}");
Console.WriteLine("Enter 'q' to quit or 'e' to change endpoint\n");

while (true)
{
    Console.Write("GraphQL Query> ");
    var queryInput = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrEmpty(queryInput)) continue;
    if (queryInput.ToLower() == "q") break;
    if (queryInput.ToLower() == "e")
    {
        Console.Write("Enter new endpoint URL: ");
        var newEndpoint = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(newEndpoint)) endpoint = newEndpoint;
        continue;
    }

    Console.Write("Variables (JSON, optional): ");
    var variablesInput = Console.ReadLine()?.Trim();
    
    try
    {
        var requestBody = BuildRequestBody(queryInput, variablesInput);
        var response = await ExecuteQuery(client, endpoint, requestBody);
        
        Console.WriteLine("\n--- Result ---");
        Console.WriteLine(PrettifyJson(response));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError: {ex.Message}\n");
    }
}

static string BuildRequestBody(string query, string? variablesInput)
{
    var request = new
    {
        query = query,
        variables = string.IsNullOrEmpty(variablesInput) 
            ? null 
            : JsonSerializer.Deserialize<Dictionary<string, object>>(variablesInput)
    };
    
    return JsonSerializer.Serialize(request);
}

static async Task<string> ExecuteQuery(HttpClient client, string endpoint, string requestBody)
{
    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
    var response = await client.PostAsync(endpoint, content);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

static string PrettifyJson(string json)
{
    try
    {
        using var doc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(doc, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }
    catch
    {
        return json;
    }
}
