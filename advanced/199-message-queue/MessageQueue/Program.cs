using System.Collections.Concurrent;

namespace MessageQueue;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Message Queue Simulator - In-memory message queue with pub/sub support");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project MessageQueue.csproj -- demo");
            Console.WriteLine("  dotnet run --project MessageQueue.csproj -- interactive");
            return 0;
        }

        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            RunDemo();
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            RunInteractiveMode();
            return 0;
        }

        Console.WriteLine($"Unknown command: {args[0]}");
        Console.WriteLine("Use 'demo' or 'interactive'");
        return 1;
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== Message Queue Simulator Demo ===\n");

        // Demo 1: Basic Queue Operations
        Console.WriteLine("1. Basic Queue Operations");
        Console.WriteLine("-------------------------");
        
        var queue = new MessageQueue();
        
        // Enqueue messages
        queue.Enqueue(new Message("order-created", "{\"orderId\": 1001, \"customer\": \"Alice\"}"));
        queue.Enqueue(new Message("order-created", "{\"orderId\": 1002, \"customer\": \"Bob\"}"));
        queue.Enqueue(new Message("order-shipped", "{\"orderId\": 1001, \"tracking\": \"ABC123\"}"));
        
        Console.WriteLine($"Queue depth: {queue.QueueDepth}");
        Console.WriteLine($"Total messages processed: {queue.Statistics.TotalEnqueued}");
        Console.WriteLine();

        // Demo 2: Topic-based Publishing
        Console.WriteLine("2. Topic-based Publishing");
        Console.WriteLine("-------------------------");
        
        var pubSub = new PubSubBroker();
        var receivedMessages = new List<(string Topic, string Message)>();
        
        // Subscribe to topics
        pubSub.Subscribe("order-*", msg =>
        {
            Console.WriteLine($"  [Subscriber 1] Received on {msg.Topic}: {msg.Content}");
            receivedMessages.Add((msg.Topic, msg.Content));
        });
        
        pubSub.Subscribe("order-created", msg =>
        {
            Console.WriteLine($"  [Subscriber 2] Order created: {msg.Content}");
        });
        
        // Publish messages
        Console.WriteLine("Publishing messages:");
        pubSub.Publish("order-created", "{\"orderId\": 1003, \"customer\": \"Charlie\"}");
        pubSub.Publish("order-shipped", "{\"orderId\": 1003, \"tracking\": \"XYZ789\"}");
        pubSub.Publish("order-cancelled", "{\"orderId\": 999}");
        Console.WriteLine();

        // Demo 3: Priority Queue
        Console.WriteLine("3. Priority Queue");
        Console.WriteLine("---------------");

        var priorityQueue = new PriorityQueue();

        var messages = new[]
        {
            ("normal-priority", "Normal task 1", MessagePriority.Normal),
            ("high-priority", "URGENT: Server down!", MessagePriority.High),
            ("low-priority", "Cleanup task", MessagePriority.Low),
            ("critical", "DATABASE DOWN!", MessagePriority.Critical),
            ("normal-priority", "Normal task 2", MessagePriority.Normal)
        };

        foreach (var (topic, content, priority) in messages)
        {
            priorityQueue.Enqueue(new Message(topic, content), priority);
        }

        Console.WriteLine("Dequeuing in priority order:");
        var priorityLabels = new Dictionary<MessagePriority, string>
        {
            [MessagePriority.Critical] = "Critical",
            [MessagePriority.High] = "High",
            [MessagePriority.Normal] = "Normal",
            [MessagePriority.Low] = "Low"
        };
        
        while (priorityQueue.HasMessages)
        {
            var msg = priorityQueue.Dequeue();
            // Infer priority from content for display
            var priorityLabel = msg.Content.Contains("URGENT") || msg.Content.Contains("DATABASE") ? "High" : "Normal";
            Console.WriteLine($"  [{priorityLabel}] {msg.Content}");
        }
        Console.WriteLine();

        // Demo 4: Dead Letter Queue
        Console.WriteLine("4. Dead Letter Queue (Failed Messages)");
        Console.WriteLine("--------------------------------------");
        
        var reliableQueue = new ReliableMessageQueue();
        
        // Process with failures
        reliableQueue.Enqueue(new Message("process-1", "Data to process"));
        reliableQueue.Enqueue(new Message("process-2", "More data"));
        reliableQueue.Enqueue(new Message("process-3", "Will fail"));
        
        var processor = new MessageProcessor(reliableQueue);
        
        Console.WriteLine("Processing messages (some will fail):");
        processor.ProcessWithRetry("process-1", msg => Console.WriteLine($"  Processed: {msg.Content}"));
        processor.ProcessWithRetry("process-2", msg => Console.WriteLine($"  Processed: {msg.Content}"));
        processor.ProcessWithRetry("process-3", msg => throw new Exception("Simulated failure"));
        
        Console.WriteLine($"\nDead letter queue size: {reliableQueue.DeadLetterCount}");
        Console.WriteLine("Messages in DLQ:");
        foreach (var dlqMsg in reliableQueue.GetDeadLetters())
        {
            Console.WriteLine($"  - {dlqMsg.Topic}: {dlqMsg.Content} (failures: {dlqMsg.FailureCount})");
        }
        Console.WriteLine();

        // Demo 5: Message Delay/Scheduling
        Console.WriteLine("5. Delayed Message Processing");
        Console.WriteLine("-----------------------------");
        
        var scheduledQueue = new ScheduledMessageQueue();
        
        Console.WriteLine($"Scheduling messages at {DateTime.Now:HH:mm:ss}:");
        scheduledQueue.Enqueue(new Message("immediate", "Process now"), TimeSpan.Zero);
        scheduledQueue.Enqueue(new Message("delayed-1", "Process in 1 second"), TimeSpan.FromSeconds(1));
        scheduledQueue.Enqueue(new Message("delayed-2", "Process in 2 seconds"), TimeSpan.FromSeconds(2));
        
        Console.WriteLine("Processing available messages:");
        var startTime = DateTime.Now;
        while (scheduledQueue.HasMessages || (DateTime.Now - startTime).TotalSeconds < 3)
        {
            var msg = scheduledQueue.DequeueIfReady();
            if (msg != null)
            {
                var delay = (DateTime.Now - startTime).TotalSeconds;
                Console.WriteLine($"  [{delay:F1}s] {msg.Content}");
            }
            Thread.Sleep(500);
        }
        Console.WriteLine();

        // Demo 6: Queue Statistics
        Console.WriteLine("6. Queue Statistics");
        Console.WriteLine("-------------------");
        
        var statsQueue = new MessageQueue();
        for (int i = 0; i < 100; i++)
        {
            statsQueue.Enqueue(new Message("stats-test", $"Message {i}"));
        }
        
        for (int i = 0; i < 75; i++)
        {
            statsQueue.Dequeue();
        }
        
        var stats = statsQueue.Statistics;
        Console.WriteLine($"Total enqueued: {stats.TotalEnqueued}");
        Console.WriteLine($"Total dequeued: {stats.TotalDequeued}");
        Console.WriteLine($"Current depth: {stats.QueueDepth}");
        Console.WriteLine($"Throughput: {stats.ThroughputPerSecond:F2} msg/sec");
        Console.WriteLine();

        Console.WriteLine("Demo completed!");
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Message Queue Simulator (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var queue = new MessageQueue();
        var pubSub = new PubSubBroker();
        var subscriptions = new Dictionary<string, string>();

        while (true)
        {
            Console.Write("mq> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "quit":
                    case "exit":
                        return;

                    case "help":
                        ShowHelp();
                        break;

                    case "enqueue":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: enqueue <topic> <content>");
                            break;
                        }
                        var topic = parts[1];
                        var content = string.Join(" ", parts.Skip(2));
                        queue.Enqueue(new Message(topic, content));
                        Console.WriteLine($"Enqueued: {topic}");
                        break;

                    case "dequeue":
                        var msg = queue.Dequeue();
                        if (msg == null)
                        {
                            Console.WriteLine("Queue is empty");
                        }
                        else
                        {
                            Console.WriteLine($"Dequeued: [{msg.Topic}] {msg.Content}");
                        }
                        break;

                    case "subscribe":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: subscribe <subscriber-id> <topic-pattern>");
                            break;
                        }
                        var subId = parts[1];
                        var pattern = parts[2];
                        subscriptions[subId] = pattern;
                        pubSub.Subscribe(pattern, msg =>
                        {
                            Console.WriteLine($"  [{subId}] Received: {msg.Content}");
                        });
                        Console.WriteLine($"Subscribed {subId} to {pattern}");
                        break;

                    case "publish":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: publish <topic> <content>");
                            break;
                        }
                        var pubTopic = parts[1];
                        var pubContent = string.Join(" ", parts.Skip(2));
                        pubSub.Publish(pubTopic, pubContent);
                        break;

                    case "depth":
                        Console.WriteLine($"Queue depth: {queue.QueueDepth}");
                        break;

                    case "stats":
                        var stats = queue.Statistics;
                        Console.WriteLine($"Enqueued: {stats.TotalEnqueued}");
                        Console.WriteLine($"Dequeued: {stats.TotalDequeued}");
                        Console.WriteLine($"Throughput: {stats.ThroughputPerSecond:F2} msg/sec");
                        break;

                    case "subs":
                        if (subscriptions.Count == 0)
                        {
                            Console.WriteLine("No subscriptions");
                        }
                        else
                        {
                            Console.WriteLine("Subscriptions:");
                            foreach (var sub in subscriptions)
                            {
                                Console.WriteLine($"  {sub.Key} -> {sub.Value}");
                            }
                        }
                        break;

                    case "clear":
                        queue = new MessageQueue();
                        pubSub = new PubSubBroker();
                        subscriptions.Clear();
                        Console.WriteLine("Cleared all queues and subscriptions");
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help' for commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  enqueue <topic> <content>  - Add message to queue");
        Console.WriteLine("  dequeue                    - Get next message");
        Console.WriteLine("  subscribe <id> <pattern>   - Subscribe to topic");
        Console.WriteLine("  publish <topic> <content>  - Publish to topic");
        Console.WriteLine("  depth                      - Show queue depth");
        Console.WriteLine("  stats                      - Show statistics");
        Console.WriteLine("  subs                       - List subscriptions");
        Console.WriteLine("  clear                      - Clear all data");
        Console.WriteLine("  quit                       - Exit");
        Console.WriteLine();
    }
}

// Message record
public record Message(string Topic, string Content, DateTime Timestamp = default, int FailureCount = 0)
{
    public DateTime Timestamp1 { get; } = Timestamp == default ? DateTime.UtcNow : Timestamp;
}

// Message priority enum
public enum MessagePriority { Low = 0, Normal = 1, High = 2, Critical = 3 }

// Queue statistics
public record QueueStatistics(
    int TotalEnqueued,
    int TotalDequeued,
    int QueueDepth,
    double ThroughputPerSecond
);

// Basic message queue
public class MessageQueue
{
    private readonly ConcurrentQueue<Message> _queue = new();
    private int _totalEnqueued;
    private int _totalDequeued;
    private DateTime _startTime = DateTime.UtcNow;

    public void Enqueue(Message message)
    {
        _queue.Enqueue(message);
        Interlocked.Increment(ref _totalEnqueued);
    }

    public Message? Dequeue()
    {
        if (_queue.TryDequeue(out var message))
        {
            Interlocked.Increment(ref _totalDequeued);
            return message;
        }
        return null;
    }

    public int QueueDepth => _queue.Count;

    public QueueStatistics Statistics
    {
        get
        {
            var elapsed = (DateTime.UtcNow - _startTime).TotalSeconds;
            return new QueueStatistics(
                Volatile.Read(ref _totalEnqueued),
                Volatile.Read(ref _totalDequeued),
                QueueDepth,
                elapsed > 0 ? Volatile.Read(ref _totalDequeued) / elapsed : 0
            );
        }
    }
}

// Pub/Sub broker
public class PubSubBroker
{
    private readonly Dictionary<string, List<Action<Message>>> _subscribers = new();
    private readonly object _lock = new();

    public void Subscribe(string topicPattern, Action<Message> handler)
    {
        lock (_lock)
        {
            if (!_subscribers.ContainsKey(topicPattern))
            {
                _subscribers[topicPattern] = new List<Action<Message>>();
            }
            _subscribers[topicPattern].Add(handler);
        }
    }

    public void Publish(string topic, string content)
    {
        var message = new Message(topic, content);
        
        lock (_lock)
        {
            foreach (var kvp in _subscribers)
            {
                if (MatchesPattern(topic, kvp.Key))
                {
                    foreach (var handler in kvp.Value)
                    {
                        handler(message);
                    }
                }
            }
        }
    }

    private static bool MatchesPattern(string topic, string pattern)
    {
        // Simple wildcard matching: * matches everything after
        if (pattern.EndsWith("*"))
        {
            var prefix = pattern[..^1];
            return topic.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
        return topic.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }
}

// Priority queue implementation
public class PriorityQueue
{
    private readonly List<(Message Message, MessagePriority Priority)> _items = new();
    private readonly object _lock = new();

    public void Enqueue(Message message, MessagePriority priority)
    {
        lock (_lock)
        {
            _items.Add((message, priority));
            _items.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
    }

    public Message Dequeue()
    {
        lock (_lock)
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");
            
            var item = _items[0];
            _items.RemoveAt(0);
            return item.Message;
        }
    }

    public bool HasMessages => _items.Count > 0;
}

// Reliable queue with dead letter support
public class ReliableMessageQueue : MessageQueue
{
    private readonly ConcurrentQueue<Message> _deadLetterQueue = new();

    public void MarkAsFailed(Message message)
    {
        var failedMessage = message with { FailureCount = message.FailureCount + 1 };
        
        if (failedMessage.FailureCount >= 3)
        {
            _deadLetterQueue.Enqueue(failedMessage);
        }
        else
        {
            Enqueue(failedMessage); // Re-queue for retry
        }
    }

    public int DeadLetterCount => _deadLetterQueue.Count;

    public IEnumerable<Message> GetDeadLetters() => _deadLetterQueue;
}

// Message processor with retry logic
public class MessageProcessor
{
    private readonly ReliableMessageQueue _queue;

    public MessageProcessor(ReliableMessageQueue queue)
    {
        _queue = queue;
    }

    public void ProcessWithRetry(string topic, Action<Message> processor)
    {
        var message = _queue.Dequeue();
        if (message == null || message.Topic != topic)
        {
            Console.WriteLine($"  No message found for topic: {topic}");
            return;
        }

        try
        {
            processor(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Failed to process: {ex.Message}");
            _queue.MarkAsFailed(message);
        }
    }
}

// Scheduled message queue
public class ScheduledMessageQueue
{
    private readonly List<(Message Message, DateTime ReadyAt)> _scheduled = new();
    private readonly object _lock = new();

    public void Enqueue(Message message, TimeSpan delay)
    {
        lock (_lock)
        {
            _scheduled.Add((message, DateTime.UtcNow + delay));
            _scheduled.Sort((a, b) => a.ReadyAt.CompareTo(b.ReadyAt));
        }
    }

    public Message? DequeueIfReady()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            for (int i = 0; i < _scheduled.Count; i++)
            {
                if (_scheduled[i].ReadyAt <= now)
                {
                    var item = _scheduled[i];
                    _scheduled.RemoveAt(i);
                    return item.Message;
                }
            }
            return null;
        }
    }

    public bool HasMessages => _scheduled.Count > 0;
}
