# Notification System Challenge

A notification dispatching system built with ASP.NET Core using Clean Architecture principles.

The application allows:
- Creating notification messages by category
- Dispatching notifications to subscribed users
- Supporting multiple notification channels (Email, SMS, Push)
- Tracking notification history and failures

# Architecture

The solution follows Clean Architecture principles:

- Domain
  - Entities (database tables for categories, messages, notificationlogs)
  - Enums (channeltype, notificationlogstatus)

- Application
  - Services (Implementation of services for categories, messages, notificationlogs, dispatcher, notificationqueue)
  - DTOs (category, message, notificationlog)
  - Abstractions (Interface services for categories, messages, notificationlogs, dispatcher)
  - Models (message sent result)

- Infrastructure
  - Persistence
	- Migrations
	- Seeders (for channels, users, categories)
	- DbContext (database context)
  - Repositories (implementation of repositories for categories, messages, notificationlogs)
  - Notification hannels (implementations of email, sms, push notification channels)
  - Queue (in-memory notification queue for handling notification jobs and hosted worker)
  
- Api
  - Controllers (for categories, messages, notificationlogs)
  - Dependency injection

# Design Decisions

## Notification Queue and Background Worker

An in-memory notification queue was implemented to decouple message creation from dispatching. `INotificationQueue` recieves the message, add to the queue and a background worker processes the queue, dispatching notifications asynchronously. This allows for better scalability and responsiveness.

Cons: if the app crashes or restarts, pending notifications in the queue will be lost. For production, a persistent queue (e.g., RabbitMQ, Azure Service Bus) would be recommended.

Worker is implemented using `BackgroundService` and runs in the same process as the API for simplicity. In a real-world scenario, it could be hosted as an Azure Function, AWS Lambda, or a separate worker service.

## Strategy Pattern

Notification channels are implemented using the Strategy Pattern through the `INotificationChannel` abstraction.

This allows:
- Adding new channels without modifying dispatcher logic
- Open/Closed Principle compliance
- Better testability

## Repository Pattern

Repositories abstract persistence concerns from business logic.

## Correlation IDs

Correlation IDs were added to messages and notification logs to support traceability across notification flows.

## Validation

Business validation is handled at the service layer to avoid relying solely on controller validation.


# Tech Stack

- ASP.NET Core - 10
- Entity Framework Core
- SQLite
- xUnit
- Moq
- FluentAssertions

# Running the Project

## Requirements

- .NET 10 SDK (10.0.204) or later

### Install MacOS

```bash
brew install dotnet
```

### Install Windows

Download and install the .NET 10 SDK from:

https://dotnet.microsoft.com/download

### Verify installation

```bash
dotnet --version
```

10.0.204 or superior should be displayed.

## Run

```bash
git clone https://github.com/xtylo/NotificationTestNet
dotnet restore
dotnet build
dotnet test
dotnet run --project Api
```

### Runned and tested

- Windows 11 x64 bit
- macOS Tahoe 26.4

# API Endpoints

## Categories

GET `/api/v1/categories`

Returns all available categories.

---

## Notification Logs

GET `/api/v1/notificationlogs`

Returns all notification logs ordered by delivery time.

---

## Messages

POST `/api/v1/messages`

Creates and dispatches a notification message.

Example request:

```json
{
  "categoryId": 1,
  "body": "Lakers won tonight"
}
```

---

# OpenAPI

Once running, go to Scalar UI: `https://localhost:{port}/scalar`

![Main UI](Docs/scalar.png)


# UI

The application includes a view at localhost:port/home/index (views/home/index.cshtml):
- sending notifications
- viewing notification history

The UI was intentionally kept lightweight using Razor Views to prioritize backend architecture and simplicity for the challenge.

![Main UI](Docs/main-ui.png)

# Tests

Run tests with:

```bash
dotnet test
```
---

# Quick Evaluation Flow

1. Run the application
2. Open Swagger/Scalar UI
3. Open the Razor UI
4. Send a notification
5. Verify notification logs are created
6. Run unit tests

# Future Improvements

Possible future enhancements:

- Retry policies with Polly
- Outbox pattern
- Distributed tracing
- Authentication/authorization
- Pagination for notification logs
- Docker support
- Integration tests


# Other commands

# Database and migrations

The application automatically creates and seeds the SQLite database on startup.

## EF Tools
Install EF Tools (required for migrations and database updates):
```
dotnet tool install --global dotnet-ef
```
## Migrations
### Add migration
```
dotnet ef migrations add InitialCreate -p Infrastructure -s Api -o Persistance/Migrations
```

### Remove migration
```
dotnet ef migrations remove -p Infrastructure -s Api
```

## Update database
```
dotnet ef database update -p Infrastructure -s Api 

```