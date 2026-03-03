# 4Pics1Word Auth API

This is the Authentication microservice for the 4Pics1Word game architecture.

## Overview
- **Framework:** ASP.NET Core Web API
- **Database:** SQLite (Development)
- **Features:** 
  - JWT Authentication
  - Roles (player, admin)
  - Admin account seeded on startup: `admin@game.com` / `Admin123!`

## How to run locally

### Prerequisites
- .NET 8 SDK

### Steps
1. Navigate to the `auth-api` project directory.
2. Run database migrations to initialize SQLite:
   ```bash
   dotnet build
   ```
   *(Note: The database is automatically migrated and seeded upon first run if it doesn't exist)*
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access the Swagger UI documentation at:
   `http://localhost:<port>/swagger`

## Environment Variables
Environment variables can be configured in `appsettings.json` or `appsettings.Development.json`.

- `ConnectionStrings:DefaultConnection`: SQLite database filename.
- `JwtSettings:Key`: Secret symmetric key used to sign the tokens.
- `JwtSettings:Issuer`: Issuer of the tokens.
- `JwtSettings:Audience`: Audience for the tokens.
