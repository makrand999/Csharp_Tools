# C# Code Formatter

C# code formatter using Roslyn demonstrating syntax rewriting and code styling. Transforms poorly formatted code into clean, consistent C# style.

## Usage

```bash
dotnet run --project CSharpCodeFormatter.csproj
```

## Example

```
=== C# Code Formatter ===

1. Original (Poorly Formatted) Code:
--------------------------------------------------
using System;using System.Collections.Generic;
using System.Linq;

namespace BadStyle{public class UserService{private List<string>_users;...
--------------------------------------------------

2. Applying Formatting Rules...

3. Formatted Code:
--------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace BadStyle
{
    public class UserService
    {
        private List<string> _users;
        private string _conn;

        public UserService()
        {
            _users = new List<string>();
            _conn = "";
        }

        public async Task<List<string>> GetUsersAsync()
        {
            await Task.Delay(100);
            return _users;
        }
        ...
    }
}
--------------------------------------------------

4. Formatting Statistics:
   Original lines: 5
   Formatted lines: 45
   Original length: 512 chars
   Formatted length: 1024 chars

5. Specific Transformations Applied:
   a) Using directives sorted and grouped
   b) Braces placed on new lines
   c) Consistent spacing around operators
   d) 4-space indentation applied
   e) Empty lines between class members

6. Before/After Comparison:
   Before: public class Test{private int x;public Test(){x=0;}...}
   After:
      public class Test
      {
          private int x;

          public Test()
          {
              x = 0;
          }

          public int GetValue()
          {
              return x;
          }
      }
```

## Concepts Demonstrated

- Roslyn `CSharpSyntaxRewriter` for code transformation
- `SyntaxNode` manipulation and replacement
- `NormalizeWhitespace()` for automatic formatting
- Custom syntax rewriters for specific rules
- Using directive sorting and organization
- Brace style enforcement
- Spacing rules around operators
- Code style analysis and issue detection
- Line length checking
- Naming convention validation
- Syntax tree traversal for analysis
- Building custom code formatters
