# Key-Value Store

An in-memory key-value store implementation with Redis-like commands, TTL support, and JSON persistence. Features thread-safe operations, automatic expiration, and support for multiple data types.

## Usage

```bash
# Run demo
dotnet run --project KeyValueStore.csproj -- demo

# Interactive mode
dotnet run --project KeyValueStore.csproj -- interactive [data-file.json]
```

## Examples

### Demo Output
```
=== Key-Value Store Demo ===

1. SET operations:
   Added 5 keys

2. GET operations:
   name = John Doe
   age = 30
   email = john@example.com
   active = True
   scores = [95, 87, 92, 88]

3. Metadata:
   Exists 'name': True
   Exists 'unknown': False
   Total keys: 5

4. KEYS (all):
   - name
   - age
   - email
   - active
   - scores

5. TTL operations:
   Set 'temp_key' with 2 second TTL
   TTL: 2 seconds
   Value: expires_soon
   After expiry - Value:

6. DELETE operation:
   Deleted 'age', exists: False
   Count after delete: 4

7. Persistence:
   Saved to demo-store.json
   Loaded from file, count: 4
   name from persisted store: John Doe
```

### Interactive Mode Commands
```
kv> set name John Doe
OK
kv> set age 30
OK
kv> set scores [1,2,3,4,5]
OK
kv> get name
John Doe
kv> get scores
[1, 2, 3, 4, 5]
kv> exists name
1
kv> keys
  name
  age
  scores
kv> count
Keys: 3
kv> del age
1
kv> save
Saved 2 keys to kvstore-data.json
kv> quit
```

## Features

- **Basic Operations**: SET, GET, DELETE, EXISTS
- **TTL Support**: Set expiration time on keys with SETEX
- **Automatic Expiration**: Background task cleans expired keys
- **Persistence**: Save/load store to JSON file
- **Multiple Types**: Strings, numbers, booleans, arrays, objects
- **Thread-Safe**: Uses ConcurrentDictionary for concurrent access
- **Redis-like Commands**: Familiar CLI interface

## Interactive Commands

| Command | Description |
|---------|-------------|
| `set <key> <value>` | Store a value |
| `get <key>` | Retrieve a value |
| `del <key>` | Delete a key |
| `exists <key>` | Check if key exists |
| `keys` | List all keys |
| `count` | Count keys |
| `ttl <key>` | Show TTL in seconds |
| `save` | Save to file |
| `load` | Load from file |
| `clear` | Clear all keys |
| `quit` | Save and exit |

## Concepts Demonstrated

- Concurrent collections (ConcurrentDictionary)
- Thread-safe operations
- TTL (Time-To-Live) implementation
- Background task for cleanup
- JSON serialization/deserialization
- Type handling with generics
- Command pattern for CLI
- Disposable resources
