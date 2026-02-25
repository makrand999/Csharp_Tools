using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleEngine;

public interface IRule<T>
{
    string Name { get; }
    int Priority { get; }
    bool Condition(T context);
    void Action(T context);
}

public class Rule<T> : IRule<T>
{
    public string Name { get; }
    public int Priority { get; }
    private readonly Func<T, bool> _condition;
    private readonly Action<T> _action;

    public Rule(string name, int priority, Func<T, bool> condition, Action<T> action)
    {
        Name = name;
        Priority = priority;
        _condition = condition;
        _action = action;
    }

    public bool Condition(T context) => _condition(context);
    public void Action(T context) => _action(context);
}

public class RuleEngine<T>
{
    private readonly List<IRule<T>> _rules = new();
    private bool _stopOnFirstMatch;

    public RuleEngine<T> SetStopOnFirstMatch(bool stop)
    {
        _stopOnFirstMatch = stop;
        return this;
    }

    public RuleEngine<T> AddRule(IRule<T> rule)
    {
        _rules.Add(rule);
        return this;
    }

    public RuleExecutionResult Execute(T context)
    {
        var result = new RuleExecutionResult(new List<string>());
        var sortedRules = _rules.OrderByDescending(r => r.Priority).ToList();

        Console.WriteLine($"  Evaluating {sortedRules.Count} rules (ordered by priority)...\n");

        foreach (var rule in sortedRules)
        {
            Console.Write($"  [{rule.Priority}] Checking '{rule.Name}'... ");
            if (rule.Condition(context))
            {
                Console.WriteLine("✓ MATCH");
                rule.Action(context);
                result.MatchedRules.Add(rule.Name);

                if (_stopOnFirstMatch)
                {
                    Console.WriteLine("  → Stopping on first match");
                    break;
                }
            }
            else
            {
                Console.WriteLine("✗ No match");
            }
        }

        return result;
    }
}

public record RuleExecutionResult(List<string> MatchedRules);

public record LoanApplication(string ApplicantId, int CreditScore, decimal Income, decimal LoanAmount, int EmploymentYears, bool HasCollateral);

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Rule Engine Demo - Loan Approval System ===\n");

        var loanEngine = new RuleEngine<LoanApplication>();

        // Auto-approve rules (highest priority)
        loanEngine.AddRule(new Rule<LoanApplication>(
            "Premium Customer Auto-Approve",
            priority: 100,
            app => app.CreditScore >= 800 && app.Income >= 100000,
            app => Console.WriteLine($"    ✅ PREMIUM: Auto-approved ${app.LoanAmount} for {app.ApplicantId}")
        ));

        loanEngine.AddRule(new Rule<LoanApplication>(
            "High Collateral Auto-Approve",
            priority: 95,
            app => app.HasCollateral && app.LoanAmount <= 50000,
            app => Console.WriteLine($"    ✅ COLLATERAL: Approved ${app.LoanAmount} with collateral")
        ));

        // Standard approval rules
        loanEngine.AddRule(new Rule<LoanApplication>(
            "Excellent Credit Approval",
            priority: 80,
            app => app.CreditScore >= 750 && app.EmploymentYears >= 2,
            app => Console.WriteLine($"    ✅ STANDARD: Approved ${app.LoanAmount} at 4% interest")
        ));

        loanEngine.AddRule(new Rule<LoanApplication>(
            "Good Credit Approval",
            priority: 70,
            app => app.CreditScore >= 680 && app.Income >= app.LoanAmount * 0.3m,
            app => Console.WriteLine($"    ✅ STANDARD: Approved ${app.LoanAmount} at 6% interest")
        ));

        loanEngine.AddRule(new Rule<LoanApplication>(
            "Moderate Credit Review",
            priority: 60,
            app => app.CreditScore >= 600 && app.EmploymentYears >= 1,
            app => Console.WriteLine($"    ⚠️  REVIEW: Manual review required for ${app.LoanAmount}")
        ));

        // Rejection rules
        loanEngine.AddRule(new Rule<LoanApplication>(
            "Low Credit Score Reject",
            priority: 50,
            app => app.CreditScore < 600,
            app => Console.WriteLine($"    ❌ REJECTED: Credit score {app.CreditScore} below minimum")
        ));

        loanEngine.AddRule(new Rule<LoanApplication>(
            "High Debt Ratio Reject",
            priority: 45,
            app => app.LoanAmount > app.Income * 5,
            app => Console.WriteLine($"    ❌ REJECTED: Loan amount exceeds 5x income")
        ));

        // Default rule (lowest priority)
        loanEngine.AddRule(new Rule<LoanApplication>(
            "Default - Manual Review",
            priority: 1,
            app => true,
            app => Console.WriteLine($"    ⏳ DEFAULT: Sending to manual underwriting")
        ));

        var applications = new List<LoanApplication>
        {
            new("APP-001", 820, 150000, 75000, 10, false),
            new("APP-002", 720, 80000, 25000, 5, false),
            new("APP-003", 650, 45000, 15000, 1, true),
            new("APP-004", 550, 30000, 10000, 0, false),
            new("APP-005", 700, 60000, 400000, 3, false),
            new("APP-006", 780, 90000, 30000, 8, true),
        };

        foreach (var app in applications)
        {
            Console.WriteLine($"\n{'=',60}");
            Console.WriteLine($"Processing Application: {app.ApplicantId}");
            Console.WriteLine($"  Credit Score: {app.CreditScore} | Income: ${app.Income:N0}");
            Console.WriteLine($"  Loan Amount: ${app.LoanAmount:N0} | Employment: {app.EmploymentYears} years | Collateral: {app.HasCollateral}");
            Console.WriteLine($"{'-',60}");

            var result = loanEngine.Execute(app);

            Console.WriteLine($"\n  Matched Rules: {string.Join(", ", result.MatchedRules)}");
        }

        Console.WriteLine($"\n{'=',60}");
        Console.WriteLine("\n--- Single Match Mode ---");
        var singleMatchEngine = new RuleEngine<LoanApplication>().SetStopOnFirstMatch(true);
        singleMatchEngine.AddRule(new Rule<LoanApplication>(
            "First Match Test",
            priority: 100,
            app => app.CreditScore >= 700,
            app => Console.WriteLine($"    → First match: Approved!")
        ));
        singleMatchEngine.AddRule(new Rule<LoanApplication>(
            "Second Rule (won't execute)",
            priority: 90,
            app => app.CreditScore >= 600,
            app => Console.WriteLine($"    → This won't print")
        ));

        var testApp = new LoanApplication("TEST-001", 750, 50000, 20000, 5, false);
        singleMatchEngine.Execute(testApp);
    }
}
