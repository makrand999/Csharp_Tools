# Contact Manager

Manage your contacts with search, tags, and export functionality.

## Usage

```bash
# Interactive mode
dotnet run --project ContactManager.csproj

# Add contact
dotnet run --project ContactManager.csproj add

# List all contacts
dotnet run --project ContactManager.csproj list

# Search contacts
dotnet run --project ContactManager.csproj search "john"

# Get contact details
dotnet run --project ContactManager.csproj get abc12345

# Delete contact
dotnet run --project ContactManager.csproj delete abc12345

# Export to CSV
dotnet run --project ContactManager.csproj export
```

## Example

```
👤 Contact Manager
==================

Commands:
  add     - Add new contact
  list    - List all contacts
  search  - Search contacts
  get     - Get contact details
  delete  - Delete contact
  export  - Export to CSV

Enter command: add

First Name: John
Last Name: Doe
Email: john.doe@example.com
Phone: 555-1234
Company: Acme Corp
Tags (comma-separated, optional): work,developer

✅ Contact added: John Doe
   ID: abc12345
```

```
$ dotnet run --project ContactManager.csproj search "Acme"

Found 1 contact(s):

[abc12345] John Doe
  Email: john.doe@example.com
  Phone: 555-1234
  Company: Acme Corp
  Tags: work, developer
```

## Features

- **Contact management** - Add, view, edit, delete contacts
- **Search** - Find by name, email, phone, company, or tags
- **Tagging** - Organize contacts with tags
- **CSV export** - Export for use in spreadsheets
- **Persistent storage** - JSON file storage

## Data Storage

Contacts are stored in `contacts.json` in the current directory.

## Export Format

CSV format compatible with Excel, Google Sheets, and other tools.

## Concepts Demonstrated

- JSON serialization
- File I/O
- LINQ search and filtering
- CSV generation
- String manipulation
- Data modeling
