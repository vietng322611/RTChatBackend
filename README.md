# RTChatBackend

A real-time chat backend built with ASP.NET Core and Redis.

## Table of Contents
- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [API Endpoints](#api-endpoints)
  - [Users](#users)
  - [Chats](#chats)
  - [Messages](#messages)
- [SignalR Hub](#signalr-hub)
- [How to Run](#how-to-run)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Run](#run)

## Overview
This is a backend project for a real-time chat application. Demo purpose only.

## Tech Stack
- **Framework:** .NET 10.0 (ASP.NET Core Web API)
- **Real-time Communication:** [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- **Data Storage & Caching:** [Redis](https://redis.io/) (using StackExchange.Redis)
- **Authentication:** Cookie-based Authentication
- **API Documentation:** OpenAPI (Swagger)

## API Endpoints

### Users
- `POST /api/users/register`: Create a new user (Body: `{ username }`)
- `POST /api/users/login`: Authenticate using a login code (Body: `{ loginCode }`)
- `POST /api/users/logout`: Sign out the current user (Requires Authentication)
- `GET /api/users/{username}`: Get user details by username (Requires Authentication)
- `GET /api/users/search?username={query}`: Search for users by partial username (Requires Authentication)

### Chats
- `GET /api/chats/`: Get all chats for the authenticated user
- `POST /api/chats/?uid2={userId}`: Create or retrieve a direct chat with another user

### Messages
- `GET /api/messages/{chatId}`: Retrieve message history for a specific chat (Requires Authentication)

## SignalR Hub
**Endpoint:** `/chatHub`

### Client-to-Server Methods
- `GetUserStatus(Guid userId)`: Request the current online status of a user.
- `SendMessage(Guid chatId, string content)`: Send a message to a specific chat.

### Server-to-Client Events
- `UserStatusChanged(string userId, string status)`: Triggered when a user goes online/offline.
- `ReceiveUserStatus(string userId, string status)`: Response to `GetUserStatus`.
- `ReceiveMessage(MessageDto message)`: Triggered when a new message is received in an active chat.

## How to Run

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Redis Server](https://redis.io/download) (Running on `localhost:6379` by default)

### Configuration
Update `appsettings.json` with your Redis connection details:
```json
{
  "Redis": {
    "ConnectionString": "localhost",
    "Ttl": 15
  }
}
```

### Run
1. **Clone the repository:**
   ```bash
   git clone https://github.com/vietng322611/RTChatBackend.git
   cd RTChatBackend
   ```
2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```
3. **Run the application:**
   ```bash
   dotnet run --project RTChatBackend.csproj
   ```
4. **Access the API Documentation:**
   Open `http://localhost:<port>/openapi/v1.json` or use the configured Swagger UI in development mode.
