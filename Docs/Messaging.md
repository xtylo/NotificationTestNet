# Messaging transport

Notification jobs are produced through one abstraction, `INotificationPublisher`
(`Application/Interfaces`), and consumed by a transport-specific hosted service.
The transport is chosen at startup via configuration — nothing in `Application` or
`Domain` depends on the broker.

## Providers

| `Messaging:Provider` | Publisher | Consumer | Notes |
|----------------------|-----------|----------|-------|
| `InMemory` (default) | `InMemoryNotificationPublisher` → `System.Threading.Channels` | `NotificationWorker` | Single process, non-durable. Good for tests. |
| `RabbitMq`           | `RabbitMqNotificationPublisher` | `RabbitMqNotificationWorker` | Durable, survives restarts, scales horizontally. |

Selection happens in `Infrastructure/DependencyInjection.cs` (`AddMessaging`).

## Running RabbitMQ locally

```bash
docker compose up -d        # starts RabbitMQ + management UI
```

- AMQP endpoint: `localhost:5672`
- Management UI: http://localhost:15672 (guest / guest)

`appsettings.Development.json` sets `Messaging:Provider = "RabbitMq"`, so running the
API in Development connects to the container above. To run fully in-process instead,
set `Messaging:Provider` to `InMemory` (or remove it).

Config block (`appsettings.json` → `Messaging:RabbitMq`) covers host, port,
credentials, queue name, dead-letter exchange/queue, and prefetch count.

## Topology & delivery semantics

`RabbitMqTopology` declares (idempotently):

- a durable work queue `notifications` with a dead-letter exchange argument;
- a dead-letter exchange `notifications.dlx` + queue `notifications.dlq`.

Jobs are published as persistent JSON with `CorrelationId` set for tracing.
The consumer acks on success and nacks (`requeue: false`) on failure, so poison
messages dead-letter to `notifications.dlq` instead of being lost or redelivered
forever.

> RabbitMQ is at-least-once: a job can be delivered more than once (e.g. ack lost on
> restart). `INotificationJobProcessor` should be idempotent — the `NotificationLog`
> table is a natural dedup anchor on `(MessageId, UserId, Channel)`.

## Moving to Azure Service Bus in prod

Add a `ServiceBusNotificationPublisher : INotificationPublisher` and a
`ServiceBusNotificationWorker` (using `ServiceBusProcessor`), then add a
`"ServiceBus"` branch to `AddMessaging`. No application-layer changes required.
