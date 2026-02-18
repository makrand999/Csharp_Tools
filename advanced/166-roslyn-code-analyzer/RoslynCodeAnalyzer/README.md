# Roslyn Code Analyzer

Roslyn-based code analyzer demonstrating syntax trees, code analysis, and compilation. Analyzes C# source code for patterns, issues, and code quality metrics.

## Usage

```bash
dotnet run --project RoslynCodeAnalyzer.csproj
```

## Example

```
=== Roslyn Code Analyzer ===

1. Syntax Tree Overview:
   Root kind: CompilationUnit
   Has errors: False
   Total nodes: 245
   Total tokens: 156

2. Classes Found:
   - UserService
     Modifiers: public
     Members: 7

3. Methods Analysis:
   - .ctor
     Return type: void
     Modifiers: public
     Parameters: 0
     Async: False
   - GetUsersAsync
     Return type: Task<List<string>>
     Modifiers: public async
     Parameters: 0
     Async: True
   - AddUser
     ...

4. Code Quality Analysis:
   ⚠️ 1 async void method(s) - avoid async void
   ⚠️ Class 'UserService' has 2 public field(s)
   ℹ️ 1 ToUpper() call(s) - consider ToUpperInvariant()
   ℹ️ 1 string.Format() call(s) - consider string interpolation
   Total issues found: 2

5. Async Method Analysis:
   GetUsersAsync: 1 await expressions
   FireAndForget: 1 await expressions
     ⚠️ WARNING: async void method detected!

6. Null Check Patterns:
   Found null check: name != null

7. Semantic Analysis:
   .ctor:
     Containing type: UserService
     Return type: void
     IsStatic: False
   GetUsersAsync:
     Containing type: UserService
     Return type: Task<List<string>>
     IsStatic: False

8. String Interpolation Opportunities:
   Found: string.Format("Message: {0}", msg)
     → Could use string interpolation instead

9. Type Declarations:
   struct Point: 2 members
   record Person: 2 parameters

10. Compilation Diagnostics:
   ✓ No compilation errors or warnings

=== Analysis Complete ===
```

## Concepts Demonstrated

- `CSharpSyntaxTree.ParseText()` for parsing source code
- Syntax tree traversal (`DescendantNodes()`, `ChildNodes()`)
- `SyntaxNode` and `SyntaxToken` inspection
- Finding specific syntax types (ClassDeclaration, MethodDeclaration, etc.)
- Semantic model for type information
- Compilation creation and diagnostics
- Code quality analysis patterns
- Async/await pattern detection
- Null check pattern detection
- String interpolation opportunity detection
- Custom code analyzer implementation
- Roslyn API for static code analysis
