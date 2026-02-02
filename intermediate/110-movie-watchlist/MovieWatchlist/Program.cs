using System.Text.Json;
using System.Text.Json.Serialization;

var moviesFile = "movies.json";
var movies = LoadMovies(moviesFile);

while (true)
{
    Console.WriteLine("\n=== Movie Watchlist ===");
    Console.WriteLine("1. Add Movie");
    Console.WriteLine("2. List Movies");
    Console.WriteLine("3. Mark as Watched");
    Console.WriteLine("4. Rate Movie");
    Console.WriteLine("5. View Statistics");
    Console.WriteLine("6. Delete Movie");
    Console.WriteLine("7. Save/Export");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1": AddMovie(movies); break;
        case "2": ListMovies(movies); break;
        case "3": MarkAsWatched(movies); break;
        case "4": RateMovie(movies); break;
        case "5": ViewStatistics(movies); break;
        case "6": DeleteMovie(movies); break;
        case "7": SaveMovies(movies, moviesFile); break;
        case "0": SaveAndExit(movies, moviesFile); return;
        default: Console.WriteLine("Invalid option."); break;
    }
}

static List<Movie> LoadMovies(string path)
{
    if (!File.Exists(path)) return [];
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<List<Movie>>(json) ?? [];
}

static void SaveMovies(List<Movie> movies, string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(path, JsonSerializer.Serialize(movies, options));
    Console.WriteLine($"Movies saved to {path}");
}

static void AddMovie(List<Movie> movies)
{
    Console.Write("Movie title: ");
    var title = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    Console.Write("Genre (e.g., Action, Comedy, Drama): ");
    var genre = Console.ReadLine()?.Trim() ?? "Unknown";

    Console.Write("Release year: ");
    if (!int.TryParse(Console.ReadLine(), out var year) || year < 1900 || year > DateTime.Now.Year + 5)
    {
        Console.WriteLine("Invalid year.");
        return;
    }

    Console.Write("Runtime (minutes): ");
    if (!int.TryParse(Console.ReadLine(), out var runtime) || runtime <= 0)
    {
        Console.WriteLine("Invalid runtime.");
        return;
    }

    Console.Write("Streaming platform (optional): ");
    var platform = Console.ReadLine()?.Trim();

    movies.Add(new Movie
    {
        Id = movies.Count > 0 ? movies.Max(m => m.Id) + 1 : 1,
        Title = title,
        Genre = genre,
        Year = year,
        RuntimeMinutes = runtime,
        StreamingPlatform = platform ?? "",
        IsWatched = false,
        AddedAt = DateTime.Now
    });

    Console.WriteLine($"Movie '{title}' added to watchlist!");
}

static void ListMovies(List<Movie> movies)
{
    if (movies.Count == 0)
    {
        Console.WriteLine("No movies in your watchlist.");
        return;
    }

    Console.WriteLine("\nFilter options:");
    Console.WriteLine("  1. All movies");
    Console.WriteLine("  2. Watchlist only");
    Console.WriteLine("  3. Watched only");
    Console.Write("Choose filter: ");
    var filterChoice = Console.ReadLine();

    var filtered = filterChoice switch
    {
        "2" => movies.Where(m => !m.IsWatched).ToList(),
        "3" => movies.Where(m => m.IsWatched).ToList(),
        _ => movies
    };

    if (filtered.Count == 0)
    {
        Console.WriteLine("No movies found with this filter.");
        return;
    }

    Console.WriteLine($"\n{"Id",-5} {"Title",-30} {"Year",-6} {"Genre",-15} {"Status"}");
    Console.WriteLine(new string('-', 75));
    foreach (var movie in filtered)
    {
        var status = movie.IsWatched
            ? $"★ Watched ({movie.Rating}/10)"
            : "○ Watchlist";
        Console.WriteLine($"{movie.Id,-5} {movie.Title,-30} {movie.Year,-6} {movie.Genre,-15} {status}");
    }
}

static void MarkAsWatched(List<Movie> movies)
{
    Console.Write("Enter movie ID to mark as watched: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var movie = movies.FirstOrDefault(m => m.Id == id);
    if (movie == null)
    {
        Console.WriteLine($"Movie with ID {id} not found.");
        return;
    }

    if (movie.IsWatched)
    {
        Console.WriteLine($"'{movie.Title}' is already marked as watched.");
        return;
    }

    movie.IsWatched = true;
    movie.WatchedAt = DateTime.Now;
    Console.WriteLine($"'{movie.Title}' marked as watched!");
}

static void RateMovie(List<Movie> movies)
{
    Console.Write("Enter movie ID to rate: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var movie = movies.FirstOrDefault(m => m.Id == id);
    if (movie == null)
    {
        Console.WriteLine($"Movie with ID {id} not found.");
        return;
    }

    if (!movie.IsWatched)
    {
        Console.WriteLine("Please mark the movie as watched before rating it.");
        return;
    }

    Console.Write("Enter rating (1-10): ");
    if (!int.TryParse(Console.ReadLine(), out var rating) || rating < 1 || rating > 10)
    {
        Console.WriteLine("Invalid rating. Please enter a number between 1 and 10.");
        return;
    }

    movie.Rating = rating;
    Console.WriteLine($"'{movie.Title}' rated {rating}/10!");
}

static void ViewStatistics(List<Movie> movies)
{
    if (movies.Count == 0)
    {
        Console.WriteLine("No movies in your watchlist.");
        return;
    }

    var total = movies.Count;
    var watched = movies.Count(m => m.IsWatched);
    var watchlist = total - watched;
    var watchedPercentage = total > 0 ? (watched / (double)total) * 100 : 0;

    var ratedMovies = movies.Where(m => m.Rating > 0).ToList();
    var avgRating = ratedMovies.Count > 0 ? ratedMovies.Average(m => m.Rating) : 0;
    var topRated = ratedMovies.OrderByDescending(m => m.Rating).Take(3).ToList();

    var totalRuntime = movies.Sum(m => m.RuntimeMinutes);
    var watchedRuntime = movies.Where(m => m.IsWatched).Sum(m => m.RuntimeMinutes);

    Console.WriteLine("\n=== Watchlist Statistics ===");
    Console.WriteLine($"Total movies:      {total}");
    Console.WriteLine($"Watched:           {watched} ({watchedPercentage:F1}%)");
    Console.WriteLine($"Watchlist:         {watchlist}");
    Console.WriteLine($"Average rating:    {avgRating:F1}/10");
    Console.WriteLine($"Total runtime:     {totalRuntime} min ({totalRuntime / 60.0:F1} hours)");
    Console.WriteLine($"Watched runtime:   {watchedRuntime} min ({watchedRuntime / 60.0:F1} hours)");

    if (topRated.Count > 0)
    {
        Console.WriteLine("\nTop rated movies:");
        foreach (var movie in topRated)
        {
            Console.WriteLine($"  ★ {movie.Title} ({movie.Year}) - {movie.Rating}/10");
        }
    }

    Console.WriteLine("\nMovies by genre:");
    var byGenre = movies.GroupBy(m => m.Genre)
                        .Select(g => new { Genre = g.Key, Count = g.Count(), Watched = g.Count(m => m.IsWatched) })
                        .OrderByDescending(x => x.Count);
    foreach (var g in byGenre)
    {
        Console.WriteLine($"  {g.Genre}: {g.Count} total, {g.Watched} watched");
    }
}

static void DeleteMovie(List<Movie> movies)
{
    Console.Write("Enter movie ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var movie = movies.FirstOrDefault(m => m.Id == id);
    if (movie == null)
    {
        Console.WriteLine($"Movie with ID {id} not found.");
        return;
    }

    Console.Write($"Delete '{movie.Title}'? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y")
    {
        Console.WriteLine("Deletion cancelled.");
        return;
    }

    movies.Remove(movie);
    Console.WriteLine($"'{movie.Title}' deleted from watchlist!");
}

static void SaveAndExit(List<Movie> movies, string path)
{
    SaveMovies(movies, path);
    Console.WriteLine("Goodbye! Happy watching!");
}

class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("genre")]
    public string Genre { get; set; } = "";

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("runtimeMinutes")]
    public int RuntimeMinutes { get; set; }

    [JsonPropertyName("streamingPlatform")]
    public string StreamingPlatform { get; set; } = "";

    [JsonPropertyName("isWatched")]
    public bool IsWatched { get; set; }

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("watchedAt")]
    public DateTime? WatchedAt { get; set; }

    [JsonPropertyName("addedAt")]
    public DateTime AddedAt { get; set; }
}
