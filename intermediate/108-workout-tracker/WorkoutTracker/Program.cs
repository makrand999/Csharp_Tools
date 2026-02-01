using System.Text.Json;
using System.Text.Json.Serialization;

var workoutsFile = "workouts.json";
var workouts = LoadWorkouts(workoutsFile);

while (true)
{
    Console.WriteLine("\n=== Workout Tracker ===");
    Console.WriteLine("1. Log Workout");
    Console.WriteLine("2. View Workouts");
    Console.WriteLine("3. View Progress by Exercise");
    Console.WriteLine("4. View Statistics");
    Console.WriteLine("5. Delete Workout");
    Console.WriteLine("6. Save/Export");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1": LogWorkout(workouts); break;
        case "2": ViewWorkouts(workouts); break;
        case "3": ViewProgress(workouts); break;
        case "4": ViewStatistics(workouts); break;
        case "5": DeleteWorkout(workouts); break;
        case "6": SaveWorkouts(workouts, workoutsFile); break;
        case "0": SaveAndExit(workouts, workoutsFile); return;
        default: Console.WriteLine("Invalid option."); break;
    }
}

static List<Workout> LoadWorkouts(string path)
{
    if (!File.Exists(path)) return [];
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<List<Workout>>(json) ?? [];
}

static void SaveWorkouts(List<Workout> workouts, string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(path, JsonSerializer.Serialize(workouts, options));
    Console.WriteLine($"Workouts saved to {path}");
}

static void LogWorkout(List<Workout> workouts)
{
    Console.Write("Date (YYYY-MM-DD, or Enter for today): ");
    var dateInput = Console.ReadLine();
    DateTime workoutDate;
    if (string.IsNullOrWhiteSpace(dateInput))
    {
        workoutDate = DateTime.Today;
    }
    else if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out workoutDate))
    {
        Console.WriteLine("Invalid date format. Use YYYY-MM-DD.");
        return;
    }

    Console.Write("Exercise name: ");
    var exercise = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(exercise))
    {
        Console.WriteLine("Exercise name cannot be empty.");
        return;
    }

    Console.Write("Sets: ");
    if (!int.TryParse(Console.ReadLine(), out var sets) || sets <= 0)
    {
        Console.WriteLine("Invalid number of sets.");
        return;
    }

    Console.Write("Reps per set: ");
    if (!int.TryParse(Console.ReadLine(), out var reps) || reps <= 0)
    {
        Console.WriteLine("Invalid number of reps.");
        return;
    }

    Console.Write("Weight (lbs/kg, or 0 for bodyweight): ");
    if (!double.TryParse(Console.ReadLine(), out var weight) || weight < 0)
    {
        Console.WriteLine("Invalid weight.");
        return;
    }

    Console.Write("Notes (optional): ");
    var notes = Console.ReadLine() ?? "";

    workouts.Add(new Workout
    {
        Id = workouts.Count > 0 ? workouts.Max(w => w.Id) + 1 : 1,
        Date = workoutDate,
        Exercise = exercise,
        Sets = sets,
        Reps = reps,
        Weight = weight,
        Notes = notes,
        LoggedAt = DateTime.Now
    });

    Console.WriteLine($"Workout logged: {exercise} - {sets}x{reps} @ {weight}");
}

static void ViewWorkouts(List<Workout> workouts)
{
    if (workouts.Count == 0)
    {
        Console.WriteLine("No workouts logged yet.");
        return;
    }

    Console.Write("Filter by exercise (or Enter for all): ");
    var filter = Console.ReadLine()?.Trim();

    var filtered = string.IsNullOrWhiteSpace(filter)
        ? workouts.OrderByDescending(w => w.Date).ThenByDescending(w => w.LoggedAt).Take(20).ToList()
        : workouts.Where(w => w.Exercise.ToLower() == filter.ToLower())
                  .OrderByDescending(w => w.Date).Take(20).ToList();

    if (filtered.Count == 0)
    {
        Console.WriteLine("No workouts found.");
        return;
    }

    Console.WriteLine($"\n{"Date",-12} {"Exercise",-20} {"Sets",-6} {"Reps",-6} {"Weight",-8} {"Notes"}");
    Console.WriteLine(new string('-', 75));
    foreach (var w in filtered)
    {
        var notesPreview = string.IsNullOrWhiteSpace(w.Notes) ? "" : w.Notes.Length > 15 ? w.Notes[..12] + "..." : w.Notes;
        Console.WriteLine($"{w.Date:yyyy-MM-dd,-12} {w.Exercise,-20} {w.Sets,-6} {w.Reps,-6} {w.Weight,-8} {notesPreview}");
    }

    if (string.IsNullOrWhiteSpace(filter) && workouts.Count > 20)
    {
        Console.WriteLine($"\nShowing last 20 of {workouts.Count} workouts. Filter to see more specific results.");
    }
}

static void ViewProgress(List<Workout> workouts)
{
    Console.Write("Exercise name: ");
    var exercise = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(exercise))
    {
        Console.WriteLine("Exercise name cannot be empty.");
        return;
    }

    var exerciseWorkouts = workouts.Where(w => w.Exercise.ToLower() == exercise.ToLower())
                                   .OrderBy(w => w.Date).ToList();

    if (exerciseWorkouts.Count == 0)
    {
        Console.WriteLine($"No workouts found for '{exercise}'.");
        return;
    }

    Console.WriteLine($"\nProgress for '{exercise}':");
    Console.WriteLine($"{"Date",-12} {"Sets",-6} {"Reps",-6} {"Weight",-8} {"Volume"}");
    Console.WriteLine(new string('-', 50));

    foreach (var w in exerciseWorkouts)
    {
        var volume = w.Sets * w.Reps * w.Weight;
        Console.WriteLine($"{w.Date:yyyy-MM-dd,-12} {w.Sets,-6} {w.Reps,-6} {w.Weight,-8} {volume}");
    }

    var first = exerciseWorkouts.First();
    var last = exerciseWorkouts.Last();
    Console.WriteLine($"\nProgress Summary:");
    Console.WriteLine($"  First: {first.Date:yyyy-MM-dd} - {first.Sets}x{first.Reps} @ {first.Weight}");
    Console.WriteLine($"  Latest: {last.Date:yyyy-MM-dd} - {last.Sets}x{last.Reps} @ {last.Weight}");
    Console.WriteLine($"  Total sessions: {exerciseWorkouts.Count}");
}

static void ViewStatistics(List<Workout> workouts)
{
    if (workouts.Count == 0)
    {
        Console.WriteLine("No workouts logged yet.");
        return;
    }

    var totalWorkouts = workouts.Count;
    var totalVolume = workouts.Sum(w => w.Sets * w.Reps * w.Weight);
    var exercises = workouts.Select(w => w.Exercise).Distinct().Count();
    var firstWorkout = workouts.Min(w => w.Date);
    var lastWorkout = workouts.Max(w => w.Date);
    var daysActive = (lastWorkout - firstWorkout).Days + 1;

    Console.WriteLine("\n=== Workout Statistics ===");
    Console.WriteLine($"Total workouts logged: {totalWorkouts}");
    Console.WriteLine($"Unique exercises: {exercises}");
    Console.WriteLine($"Total volume (sets×reps×weight): {totalVolume:N0}");
    Console.WriteLine($"First workout: {firstWorkout:yyyy-MM-dd}");
    Console.WriteLine($"Latest workout: {lastWorkout:yyyy-MM-dd}");
    Console.WriteLine($"Days active: {daysActive}");
    Console.WriteLine($"Average workouts per week: {(totalWorkouts / Math.Max(1, daysActive / 7.0)):F1}");

    Console.WriteLine("\nTop exercises by volume:");
    var topExercises = workouts.GroupBy(w => w.Exercise)
                               .Select(g => new { Exercise = g.Key, Volume = g.Sum(w => w.Sets * w.Reps * w.Weight) })
                               .OrderByDescending(x => x.Volume)
                               .Take(5);
    foreach (var ex in topExercises)
    {
        Console.WriteLine($"  {ex.Exercise}: {ex.Volume:N0}");
    }
}

static void DeleteWorkout(List<Workout> workouts)
{
    Console.Write("Enter workout ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var workout = workouts.FirstOrDefault(w => w.Id == id);
    if (workout == null)
    {
        Console.WriteLine($"Workout with ID {id} not found.");
        return;
    }

    Console.Write($"Delete '{workout.Exercise}' on {workout.Date:yyyy-MM-dd}? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y")
    {
        Console.WriteLine("Deletion cancelled.");
        return;
    }

    workouts.Remove(workout);
    Console.WriteLine("Workout deleted successfully!");
}

static void SaveAndExit(List<Workout> workouts, string path)
{
    SaveWorkouts(workouts, path);
    Console.WriteLine("Goodbye! Keep training!");
}

class Workout
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("exercise")]
    public string Exercise { get; set; } = "";

    [JsonPropertyName("sets")]
    public int Sets { get; set; }

    [JsonPropertyName("reps")]
    public int Reps { get; set; }

    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = "";

    [JsonPropertyName("loggedAt")]
    public DateTime LoggedAt { get; set; }
}
