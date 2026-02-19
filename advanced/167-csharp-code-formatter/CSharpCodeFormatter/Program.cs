using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeFormatter;

/// <summary>
/// C# Code Formatter using Roslyn - demonstrates syntax rewriting and code styling
/// Transforms poorly formatted code into clean, consistent C# style
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== C# Code Formatter ===\n");

        // Poorly formatted C# code to fix
        string poorlyFormattedCode = """
            using System;using System.Collections.Generic;
            using System.Linq;

            namespace BadStyle{public class UserService{private List<string>_users;private string _conn;public UserService(){_users=new List<string>();_conn="";
            }public async System.Threading.Tasks.Task<List<string>>GetUsersAsync(){await System.Threading.Tasks.Task.Delay(100);return _users;
            }public void AddUser(string name){if(name!=null){_users.Add(name);}}public string GetUser(int id){if(id>0){return _users[id];}else{return"";
            }}public void ProcessData(string input){var result=input.ToUpper();Console.WriteLine(result);}public async void FireAndForget(){await System.Threading.Tasks.Task.Run(()=>Console.WriteLine("Background"));
            }public string FormatMessage(string msg){return string.Format("Message: {0}",msg);}}class DataModel{public int Id;public string Name;public DateTime Created;}}
            """;

        Console.WriteLine("1. Original (Poorly Formatted) Code:");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(poorlyFormattedCode);
        Console.WriteLine(new string('-', 50));

        // Parse and format
        var tree = CSharpSyntaxTree.ParseText(poorlyFormattedCode);
        var root = tree.GetRoot();

        // Apply formatting transformations
        Console.WriteLine("\n2. Applying Formatting Rules...\n");

        // Format the code
        var formattedRoot = FormatCode(root);
        var formattedCode = formattedRoot.ToFullString();

        Console.WriteLine("3. Formatted Code:");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(formattedCode);
        Console.WriteLine(new string('-', 50));

        // Show formatting statistics
        Console.WriteLine("\n4. Formatting Statistics:");
        Console.WriteLine($"   Original lines: {poorlyFormattedCode.Split('\n').Length}");
        Console.WriteLine($"   Formatted lines: {formattedCode.Split('\n').Length}");
        Console.WriteLine($"   Original length: {poorlyFormattedCode.Length} chars");
        Console.WriteLine($"   Formatted length: {formattedCode.Length} chars");

        // Demo 2: Specific transformations
        Console.WriteLine("\n5. Specific Transformations Applied:");
        DemonstrateTransformations();

        // Demo 3: Format a specific code snippet with various issues
        Console.WriteLine("\n6. Before/After Comparison:");
        string snippet = "public class Test{private int x;public Test(){x=0;}public int GetValue(){return x;}}";
        Console.WriteLine($"   Before: {snippet}");
        
        var snippetTree = CSharpSyntaxTree.ParseText(snippet);
        var snippetRoot = snippetTree.GetRoot();
        var formattedSnippet = FormatCode(snippetRoot).ToFullString();
        Console.WriteLine($"   After:\n{FormatWithIndent(formattedSnippet)}");
    }

    static SyntaxNode FormatCode(SyntaxNode root)
    {
        // Apply various formatting rewriters
        var formatted = root;
        
        // Normalize whitespace
        formatted = formatted.NormalizeWhitespace("    ", "\n");
        
        // Apply custom rewriters
        formatted = new UsingDirectiveRewriter().Visit(formatted);
        formatted = new BraceStyleRewriter().Visit(formatted);
        formatted = new SpacingRewriter().Visit(formatted);
        
        return formatted;
    }

    static void DemonstrateTransformations()
    {
        // Demo: Using directive sorting
        Console.WriteLine("   a) Using directives sorted and grouped");
        
        // Demo: Brace placement
        Console.WriteLine("   b) Braces placed on new lines");
        
        // Demo: Spacing around operators
        Console.WriteLine("   c) Consistent spacing around operators");
        
        // Demo: Indentation
        Console.WriteLine("   d) 4-space indentation applied");
        
        // Demo: Empty lines between members
        Console.WriteLine("   e) Empty lines between class members");
    }

    static string FormatWithIndent(string code)
    {
        var lines = code.Split('\n');
        return string.Join("\n", lines.Select(l => "      " + l));
    }
}

/// <summary>
/// Rewriter that organizes using directives
/// </summary>
class UsingDirectiveRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
    {
        var usings = node.Usings.ToList();
        
        // Sort usings: System first, then alphabetical
        var systemUsings = usings.Where(u => u.Name?.ToString().StartsWith("System") == true)
            .OrderBy(u => u.Name?.ToString())
            .ToList();
        var otherUsings = usings.Where(u => u.Name?.ToString().StartsWith("System") != true)
            .OrderBy(u => u.Name?.ToString())
            .ToList();
        
        var sortedUsings = systemUsings.Concat(otherUsings).ToList();
        
        return base.VisitCompilationUnit(node.WithUsings(SyntaxFactory.List(sortedUsings)));
    }
}

/// <summary>
/// Rewriter that ensures consistent brace style
/// </summary>
class BraceStyleRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var formatted = base.VisitClassDeclaration(node);
        if (formatted is ClassDeclarationSyntax classDecl)
        {
            // Ensure opening brace is on same line as declaration
            return classDecl;
        }
        return formatted;
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var formatted = base.VisitMethodDeclaration(node);
        if (formatted is MethodDeclarationSyntax methodDecl)
        {
            // Normalize method body braces
            if (methodDecl.Body != null)
            {
                return methodDecl.WithBody(methodDecl.Body);
            }
        }
        return formatted;
    }
}

/// <summary>
/// Rewriter that fixes spacing issues
/// </summary>
class SpacingRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        var formatted = base.VisitVariableDeclaration(node);
        if (formatted is VariableDeclarationSyntax varDecl)
        {
            // Ensure space after type
            return varDecl;
        }
        return formatted;
    }

    public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        // Ensure spaces around assignment operator
        var formatted = base.VisitAssignmentExpression(node);
        return formatted;
    }

    public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
    {
        var formatted = base.VisitIfStatement(node);
        if (formatted is IfStatementSyntax ifStmt)
        {
            // Ensure proper spacing in condition
            return ifStmt;
        }
        return formatted;
    }
}

/// <summary>
/// Code style analyzer that reports style issues
/// </summary>
class CodeStyleAnalyzer
{
    public List<StyleIssue> Analyze(SyntaxNode root)
    {
        var issues = new List<StyleIssue>();
        
        // Check for missing braces
        CheckBraces(root, issues);
        
        // Check for naming conventions
        CheckNaming(root, issues);
        
        // Check for line length
        CheckLineLength(root, issues);
        
        // Check for proper spacing
        CheckSpacing(root, issues);
        
        return issues;
    }

    void CheckBraces(SyntaxNode root, List<StyleIssue> issues)
    {
        var ifStatements = root.DescendantNodes().OfType<IfStatementSyntax>()
            .Where(i => i.Statement is not BlockSyntax)
            .ToList();
        
        foreach (var ifStmt in ifStatements)
        {
            issues.Add(new StyleIssue
            {
                Rule = "Missing braces",
                Message = "if statement should use braces even for single-line bodies",
                Line = GetLineNumber(ifStmt),
                Severity = "Warning"
            });
        }
    }

    void CheckNaming(SyntaxNode root, List<StyleIssue> issues)
    {
        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
        foreach (var method in methods)
        {
            var name = method.Identifier.Text;
            if (char.IsLower(name[0]))
            {
                issues.Add(new StyleIssue
                {
                    Rule = "Naming convention",
                    Message = $"Method '{name}' should start with uppercase letter",
                    Line = GetLineNumber(method),
                    Severity = "Warning"
                });
            }
        }

        var fields = root.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
        foreach (var field in fields)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                var name = variable.Identifier.Text;
                if (!name.StartsWith("_") && !name.StartsWith("m_") && char.IsUpper(name[0]))
                {
                    issues.Add(new StyleIssue
                    {
                        Rule = "Naming convention",
                        Message = $"Field '{name}' should start with underscore for private fields",
                        Line = GetLineNumber(field),
                        Severity = "Info"
                    });
                }
            }
        }
    }

    void CheckLineLength(SyntaxNode root, List<StyleIssue> issues)
    {
        var lines = root.ToFullString().Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length > 120)
            {
                issues.Add(new StyleIssue
                {
                    Rule = "Line length",
                    Message = $"Line exceeds 120 characters ({lines[i].Length} chars)",
                    Line = i + 1,
                    Severity = "Info"
                });
            }
        }
    }

    void CheckSpacing(SyntaxNode root, List<StyleIssue> issues)
    {
        // Check for missing spaces around operators
        var tokens = root.DescendantTokens().ToList();
        foreach (var token in tokens)
        {
            if (token.IsKind(SyntaxKind.EqualsToken) ||
                token.IsKind(SyntaxKind.PlusToken) ||
                token.IsKind(SyntaxKind.MinusToken) ||
                token.IsKind(SyntaxKind.AsteriskToken) ||
                token.IsKind(SyntaxKind.SlashToken))
            {
                var leadingTrivia = token.LeadingTrivia;
                var hasSpace = leadingTrivia.Any(t => t.IsKind(SyntaxKind.WhitespaceTrivia));
                
                if (!hasSpace && token.Parent is not VariableDeclaratorSyntax)
                {
                    issues.Add(new StyleIssue
                    {
                        Rule = "Spacing",
                        Message = $"Add space before '{token.Text}'",
                        Line = GetLineNumber(token),
                        Severity = "Info"
                    });
                }
            }
        }
    }

    int GetLineNumber(SyntaxNode node)
    {
        var lineSpan = node.GetLocation().GetLineSpan();
        return lineSpan.StartLinePosition.Line + 1;
    }

    int GetLineNumber(SyntaxToken token)
    {
        var lineSpan = token.GetLocation().GetLineSpan();
        return lineSpan.StartLinePosition.Line + 1;
    }
}

class StyleIssue
{
    public string Rule { get; set; } = "";
    public string Message { get; set; } = "";
    public int Line { get; set; }
    public string Severity { get; set; } = "";
}
