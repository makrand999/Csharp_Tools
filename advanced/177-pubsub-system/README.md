# Pub/Sub System

Publish-Subscribe messaging pattern implementation with topic-based routing. Decouples message producers from consumers.

## Usage

```bash
dotnet run --project 177-pubsub-system/PubSubSystem.csproj
```

## Example

```
Pub/Sub Messaging System
========================

--- Subscribers Registered ---

  Email Service subscribed to 'orders'
  SMS Service subscribed to 'orders'
  Analytics Service subscribed to 'payments'
  Analytics Service subscribed to 'orders'

--- Publishing Messages ---

  Publishing to 'orders': OrderCreatedEvent { OrderId = 1001, Amount = 99.99 }
  [Email Service] Received on 'orders': OrderCreatedEvent { OrderId = 1001, Amount = 99.99 }
  [SMS Service] Received on 'orders': OrderCreatedEvent { OrderId = 1001, Amount = 99.99 }
  [Analytics Service] Received on 'orders': OrderCreatedEvent { OrderId = 1001, Amount = 99.99 }

  Publishing to 'payments': PaymentProcessedEvent { OrderId = 1001, Status = Success }
  [Email Service] Received on 'payments': PaymentProcessedEvent { OrderId = 1001, Status = Success }

--- Subscriber Statistics ---
Email Service received: 4 messages
SMS Service received: 2 messages
Analytics Service received: 3 messages
```

## Components

| Component | Role |
|-----------|------|
| **MessageBroker** | Central hub for pub/sub routing |
| **ISubscriber** | Message consumer interface |
| **Topics** | Named channels for message routing |

## Concepts Demonstrated

- Publish-subscribe pattern
- Topic-based routing
- Loose coupling
- Async message delivery
- Subscription management
- Event-driven architecture
