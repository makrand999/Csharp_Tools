using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynCodeAnalyzer;

/// <summary>
/// Roslyn-based code analyzer - demonstrates syntax trees, code analysis, and compilation
/// Analyzes C# source code for patterns, issues, and code quality metrics
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Roslyn Code Analyzer ===\n");

        // Sample C# code to analyze
        string sourceCode = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;

            namespace SampleApp
            {
                public class UserService
                {
                    private List<string> _users;
                    private string _connectionString = "";
                    
                    public UserService()
                    {
                        _users = new List<string>();
                    }
                    
                    public async Task<List<string>> GetUsersAsync()
                    {
                        await Task.Delay(100);
                        return _users;
                    }
                    
                    public void AddUser(string name)
                    {
                        if (name != null)
                        {
                            _users.Add(name);
                        }
                    }
                    
                    public string GetUser(int id)
                    {
                        if (id > 0)
                        {
                            return _users[id];
                        }
                        else
                        {
                            return "";
                        }
                    }
                    
                    public void ProcessData(string input)
                    {
                        var result = input.ToUpper();
                        Console.WriteLine(result);
                    }
                    
                    public async void FireAndForget()
                    {
                        await Task.Run(() => Console.WriteLine("Background"));
                    }
                    
                    public string FormatMessage(string msg)
                    {
                        return string.Format("Message: {0}", msg);
                    }
                }
                
                public struct Point
                {
                    public int X;
                    public int Y;
                }
                
                public record Person(string Name, int Age);
            }
            """;

        // Parse the source code
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var compilation = CSharpCompilation.Create("AnalysisTarget")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
            .AddSyntaxTrees(tree);

        var root = tree.GetRoot();
        var semanticModel = compilation.GetSemanticModel(tree);

        // Demo 1: Syntax tree visualization
        Console.WriteLine("1. Syntax Tree Overview:");
        Console.WriteLine($"   Root kind: {root.Kind()}");
        Console.WriteLine($"   Has errors: {tree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)}");
        Console.WriteLine($"   Total nodes: {CountNodes(root)}");
        Console.WriteLine($"   Total tokens: {root.DescendantTokens().Count()}");

        // Demo 2: Find all classes
        Console.WriteLine("\n2. Classes Found:");
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        foreach (var classDecl in classDeclarations)
        {
            Console.WriteLine($"   - {classDecl.Identifier.Text}");
            Console.WriteLine($"     Modifiers: {string.Join(" ", classDecl.Modifiers.Select(m => m.Text))}");
            Console.WriteLine($"     Members: {classDecl.Members.Count}");
        }

        // Demo 3: Find all methods
        Console.WriteLine("\n3. Methods Analysis:");
        var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
        foreach (var method in methodDeclarations)
        {
            var modifiers = string.Join(" ", method.Modifiers.Select(m => m.Text));
            Console.WriteLine($"   - {method.Identifier.Text}");
            Console.WriteLine($"     Return type: {method.ReturnType}");
            Console.WriteLine($"     Modifiers: {(string.IsNullOrEmpty(modifiers) ? "none" : modifiers)}");
            Console.WriteLine($"     Parameters: {method.ParameterList.Parameters.Count}");
            Console.WriteLine($"     Async: {method.Modifiers.Any(m => m.Text == "async")}");
        }

        // Demo 4: Code analysis - detect issues
        Console.WriteLine("\n4. Code Quality Analysis:");
        AnalyzeCodeQuality(root, methodDeclarations);

        // Demo 5: Find async methods without await
        Console.WriteLine("\n5. Async Method Analysis:");
        var asyncMethods = methodDeclarations.Where(m => m.Modifiers.Any(m => m.Text == "async")).ToList();
        foreach (var method in asyncMethods)
        {
            var awaitExpressions = method.DescendantNodes().OfType<AwaitExpressionSyntax>().ToList();
            Console.WriteLine($"   {method.Identifier.Text}: {awaitExpressions.Count} await expressions");
            
            // Check for async void
            if (method.ReturnType is PredefinedTypeSyntax predefined && 
                predefined.Keyword.Text == "void")
            {
                Console.WriteLine($"     ⚠️ WARNING: async void method detected!");
            }
        }

        // Demo 6: Find null checks
        Console.WriteLine("\n6. Null Check Patterns:");
        var ifStatements = root.DescendantNodes().OfType<IfStatementSyntax>().ToList();
        foreach (var ifStmt in ifStatements)
        {
            var conditionText = ifStmt.Condition.ToString();
            if (conditionText.Contains("null"))
            {
                Console.WriteLine($"   Found null check: {conditionText}");
            }
        }

        // Demo 7: Type analysis with semantic model
        Console.WriteLine("\n7. Semantic Analysis:");
        foreach (var method in methodDeclarations.Take(3))
        {
            var symbol = semanticModel.GetDeclaredSymbol(method);
            if (symbol != null)
            {
                Console.WriteLine($"   {method.Identifier.Text}:");
                Console.WriteLine($"     Containing type: {symbol.ContainingType?.Name}");
                Console.WriteLine($"     Return type: {symbol.ReturnType}");
                Console.WriteLine($"     IsStatic: {symbol.IsStatic}");
            }
        }

        // Demo 8: Find string interpolation opportunities
        Console.WriteLine("\n8. String Interpolation Opportunities:");
        var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();
        foreach (var invocation in invocations)
        {
            if (invocation.Expression.ToString().Contains("string.Format") ||
                invocation.Expression.ToString().Contains("Console.WriteLine"))
            {
                Console.WriteLine($"   Found: {invocation}");
                Console.WriteLine($"     → Could use string interpolation instead");
            }
        }

        // Demo 9: Find struct vs class usage
        Console.WriteLine("\n9. Type Declarations:");
        var structs = root.DescendantNodes().OfType<StructDeclarationSyntax>().ToList();
        var records = root.DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();
        
        foreach (var structDecl in structs)
        {
            Console.WriteLine($"   struct {structDecl.Identifier.Text}: {structDecl.Members.Count} members");
        }
        foreach (var recordDecl in records)
        {
            Console.WriteLine($"   record {recordDecl.Identifier.Text}: {recordDecl.ParameterList?.Parameters.Count ?? 0} parameters");
        }

        // Demo 10: Compilation diagnostics
        Console.WriteLine("\n10. Compilation Diagnostics:");
        var diagnostics = compilation.GetDiagnostics();
        if (diagnostics.IsEmpty)
        {
            Console.WriteLine("   ✓ No compilation errors or warnings");
        }
        else
        {
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine($"   [{diagnostic.Severity}] {diagnostic.Id}: {diagnostic.GetMessage()}");
            }
        }

        Console.WriteLine("\n=== Analysis Complete ===");
    }

    static int CountNodes(SyntaxNode node)
    {
        int count = 1;
        foreach (var child in node.ChildNodes())
        {
            count += CountNodes(child);
        }
        return count;
    }

    static void AnalyzeCodeQuality(SyntaxNode root, List<MethodDeclarationSyntax> methods)
    {
        int issues = 0;

        // Check for async void
        var asyncVoidMethods = methods.Where(m => 
            m.Modifiers.Any(m => m.Text == "async") &&
            m.ReturnType is PredefinedTypeSyntax p && p.Keyword.Text == "void"
        ).ToList();
        
        if (asyncVoidMethods.Any())
        {
            Console.WriteLine($"   ⚠️ {asyncVoidMethods.Count} async void method(s) - avoid async void");
            issues++;
        }

        // Check for public fields
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        foreach (var cls in classes)
        {
            var publicFields = cls.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(f => f.Modifiers.Any(m => m.Text == "public"))
                .ToList();
            
            if (publicFields.Any())
            {
                Console.WriteLine($"   ⚠️ Class '{cls.Identifier.Text}' has {publicFields.Count} public field(s)");
                issues++;
            }
        }

        // Check for non-async methods with Async suffix
        var nonAsyncWithAsyncSuffix = methods.Where(m =>
            !m.Modifiers.Any(m => m.Text == "async") &&
            m.Identifier.Text.EndsWith("Async")
        ).ToList();
        
        if (nonAsyncWithAsyncSuffix.Any())
        {
            Console.WriteLine($"   ⚠️ {nonAsyncWithAsyncSuffix.Count} method(s) with 'Async' suffix but not async");
            issues++;
        }

        // Check for ToUpper without invariant culture
        var toUpperCalls = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(i => i.Expression.ToString().EndsWith(".ToUpper()"))
            .ToList();
        
        if (toUpperCalls.Any())
        {
            Console.WriteLine($"   ℹ️ {toUpperCalls.Count} ToUpper() call(s) - consider ToUpperInvariant()");
        }

        // Check for string.Format usage
        var stringFormatCalls = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(i => i.Expression.ToString().Contains("string.Format"))
            .ToList();
        
        if (stringFormatCalls.Any())
        {
            Console.WriteLine($"   ℹ️ {stringFormatCalls.Count} string.Format() call(s) - consider string interpolation");
        }

        Console.WriteLine($"   Total issues found: {issues}");
    }
}
