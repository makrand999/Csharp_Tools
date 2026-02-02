# Movie Watchlist

A CLI application for tracking movies you want to watch, with ratings and watch status management.

## Usage

```bash
dotnet run --project MovieWatchlist/MovieWatchlist.csproj
```

## Features

- **Add movies** with title, genre, year, runtime, and streaming platform
- **List movies** with filter options (all, watchlist, watched)
- **Mark movies as watched** with timestamp
- **Rate movies** on a 1-10 scale
- **View statistics** including completion rate, average rating, and top picks
- **Genre breakdown** showing distribution of your collection
- **Delete movies** with confirmation
- **Auto-save** to JSON file

## Example

```
=== Movie Watchlist ===
1. Add Movie
2. List Movies
3. Mark as Watched
4. Rate Movie
5. View Statistics
6. Delete Movie
7. Save/Export
0. Exit
Choose option: 1

Movie title: Inception
Genre: Sci-Fi
Release year: 2010
Runtime (minutes): 148
Streaming platform (optional): Netflix
Movie 'Inception' added to watchlist!
```

## Concepts Demonstrated

- JSON serialization for data persistence
- Nullable types (DateTime?)
- Boolean state management
- LINQ filtering, grouping, and aggregation
- Statistics calculation (averages, percentages)
- Conditional logic based on state
- Interactive CLI with multi-step workflows
