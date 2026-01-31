# Contact Manager

A CLI contact book application for managing contacts with search and tag filtering capabilities.

## Usage

```bash
dotnet run --project ContactManager/ContactManager.csproj
```

## Features

- **Add contacts** with name, phone, email, and tags
- **List all contacts** in a formatted table
- **Search contacts** by name, phone, email, or tags
- **Filter by tag** to find contacts with specific labels
- **Delete contacts** by ID
- **Auto-save** to JSON file on exit

## Example

```
=== Contact Manager ===
1. Add Contact
2. List Contacts
3. Search Contacts
4. Filter by Tag
5. Delete Contact
6. Export to JSON
0. Exit
Choose option: 1

Name: John Doe
Phone: 555-1234
Email: john@example.com
Tags (comma-separated): work, developer
Contact 'John Doe' added successfully!
```

## Concepts Demonstrated

- JSON serialization/deserialization with System.Text.Json
- File I/O for data persistence
- LINQ for searching and filtering
- Interactive CLI menu system
- Collections and list operations
- String manipulation and parsing
