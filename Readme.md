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
  - Services (Implementation of services for categories, messages, notificationlogs, dispatcher)
  - DTOs (category, message, notificationlog)
  - Abstractions (Interface services for categories, messages, notificationlogs, dispatcher)
  - Models (message sent result)

- Infrastructure
  - Persistance
	- Migrations
	- Seeders (for channels, users, categories)
	- DbContext (database context)
  - Repositories (implementation of repositories for categories, messages, notificationlogs)
  - Notification channels (implementations of email, sms, push notification channels)
  

- Api
  - Controllers (for categories, messages, notificationlogs)
  - Dependency injection

# Design Decisions

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

- .NET 10 SDK (10.0.204)

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

## Run

```bash
dotnet restore

dotnet build

dotnet run --project Api
```

# API Endpoints

## Categories

GET `/api/categories`

Returns all available categories.

---

## Messages

POST `/api/messages`

Creates and dispatches a notification message.

Example request:

```json
{
  "categoryId": 1,
  "body": "Lakers won tonight"
}
```

---

# UI

The application includes a view at localhost:port/home/index (views/home/index.cshtml):
- sending notifications
- viewing notification history

# Tests

Run tests with:

```bash
dotnet test
```
---

# Future Improvements

Possible future enhancements:

- Background queue processing
- Retry policies with Polly
- Outbox pattern
- Distributed tracing
- Authentication/authorization
- Pagination for notification logs
- Docker support
- Integration tests


# Other commands

# Database and migrations

## EF Tools
Install EF Tools
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