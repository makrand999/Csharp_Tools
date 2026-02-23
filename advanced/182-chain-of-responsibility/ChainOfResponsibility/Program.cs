using System;
using System.Collections.Generic;

namespace ChainOfResponsibility;

public record SupportTicket(int Id, string Issue, int Priority, decimal RefundAmount);

public abstract class SupportHandler
{
    protected SupportHandler? _next;

    public SupportHandler SetNext(SupportHandler next)
    {
        _next = next;
        return next;
    }

    public virtual void Handle(SupportTicket ticket)
    {
        if (_next != null)
            _next.Handle(ticket);
        else
            Console.WriteLine($"  Ticket #{ticket.Id}: No handler available, escalating to management.");
    }
}

public class Level1Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.Priority <= 2 && ticket.RefundAmount <= 50)
        {
            Console.WriteLine($"  Ticket #{ticket.Id}: Level 1 handling - '{ticket.Issue}' (Priority: {ticket.Priority}, Refund: ${ticket.RefundAmount})");
            Console.WriteLine($"    → Resolved by Level 1 support with standard solution.");
        }
        else
        {
            Console.WriteLine($"  Ticket #{ticket.Id}: Level 1 passing to Level 2 (Priority: {ticket.Priority}, Refund: ${ticket.RefundAmount})");
            base.Handle(ticket);
        }
    }
}

public class Level2Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.Priority <= 4 && ticket.RefundAmount <= 200)
        {
            Console.WriteLine($"  Ticket #{ticket.Id}: Level 2 handling - '{ticket.Issue}' (Priority: {ticket.Priority}, Refund: ${ticket.RefundAmount})");
            Console.WriteLine($"    → Resolved by Level 2 support with advanced troubleshooting.");
        }
        else
        {
            Console.WriteLine($"  Ticket #{ticket.Id}: Level 2 passing to Manager (Priority: {ticket.Priority}, Refund: ${ticket.RefundAmount})");
            base.Handle(ticket);
        }
    }
}

public class ManagerSupport : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.RefundAmount <= 1000)
        {
            Console.WriteLine($"  Ticket #{ticket.Id}: Manager handling - '{ticket.Issue}' (Priority: {ticket.Priority}, Refund: ${ticket.RefundAmount})");
            Console.WriteLine($"    → Resolved by manager with special approval.");
        }
        else
        {
            Console.WriteLine($"  Ticket #{ticket.Id}: Manager escalating to Executive (Refund: ${ticket.RefundAmount} exceeds limit)");
            base.Handle(ticket);
        }
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Chain of Responsibility Pattern Demo ===\n");

        var level1 = new Level1Support();
        var level2 = new Level2Support();
        var manager = new ManagerSupport();

        level1.SetNext(level2).SetNext(manager);

        var tickets = new List<SupportTicket>
        {
            new(1001, "Password reset", 1, 0),
            new(1002, "Billing question", 2, 25),
            new(1003, "Feature request", 3, 0),
            new(1004, "Refund request", 3, 150),
            new(1005, "Critical bug", 5, 500),
            new(1006, "Enterprise issue", 5, 2000),
        };

        foreach (var ticket in tickets)
        {
            Console.WriteLine($"\nProcessing Ticket #{ticket.Id}:");
            level1.Handle(ticket);
        }
    }
}
