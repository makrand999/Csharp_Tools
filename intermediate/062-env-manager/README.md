# Environment Variable Manager

CLI tool for managing, viewing, exporting, and importing environment variables.

## Usage

```bash
# List all variables
dotnet run --project EnvManager/EnvManager.csproj list

# List variables with filter
dotnet run --project EnvManager/EnvManager.csproj list PATH

# Get specific variable
dotnet run --project EnvManager/EnvManager.csproj get HOME

# Set variable (current process)
dotnet run --project EnvManager/EnvManager.csproj set MY_VAR value

# Export to JSON file
dotnet run --project EnvManager/EnvManager.csproj export env.json

# Import from JSON file
dotnet run --project EnvManager/EnvManager.csproj import env.json
```

## Example

```
$ dotnet run --project EnvManager/EnvManager.csproj list PATH
Found 5 environment variables:

HOME                           = /home/user
PATH                           = /usr/bin:/bin:/usr/sbin...
USER                           = max
SHELL                          = /bin/bash
PWD                            = /home/max/learn/csharp/300
```

## Concepts Demonstrated

- Environment variable manipulation
- Command-line argument parsing
- JSON serialization for data export
- File I/O operations
- LINQ filtering and sorting
