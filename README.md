# Pokemon Review API

A learning project built with ASP.NET Core that manages Pokémon, owners, categories, reviewers, and reviews. It demonstrates REST controllers, dependency injection, Entity Framework Core relationships, repository interfaces, and DTO mapping.

The source project and namespaces retain the original `YouTubeWebAPI` name.

## Stack

- C# / .NET 8 / ASP.NET Core
- Entity Framework Core 8 with SQL Server
- AutoMapper and Swagger / OpenAPI
- xUnit with an in-memory SQLite database for rating regression tests

## Run locally

Prerequisites: .NET 8 SDK and a running SQL Server instance accessible from your machine. SQL Server may run separately or in a compatible container. Linux users need a SQL Server connection; Windows LocalDB is not required.

Run these commands from the repository root. Use a local development database and replace the placeholders with your server details.

```bash
dotnet restore YouTubeWebAPI.sln
dotnet user-secrets init --project YouTubeWebAPI
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=PokemonReview;User Id=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=True" \
  --project YouTubeWebAPI
dotnet tool install --global dotnet-ef --version '8.0.*'
dotnet ef database update --project YouTubeWebAPI
dotnet run --project YouTubeWebAPI --launch-profile http
```

If `dotnet-ef` is already installed, use `dotnet tool update` instead. On Linux, ensure `$HOME/.dotnet/tools` is on your `PATH`. The certificate setting above is for a local development server.

Open **http://localhost:5106/swagger** to explore the endpoints.

To insert sample data, stop the server and run:

```bash
dotnet run --project YouTubeWebAPI --launch-profile http -- seeddata
```

The database migrations must already be applied. This command seeds sample Pokémon when the owner relationship table is empty, then starts the server.

## Pokémon endpoints

| Method | Path | Purpose |
| --- | --- | --- |
| GET | `/api/Pokemon` | List Pokémon |
| GET | `/api/Pokemon/{id}` | Get one Pokémon |
| GET | `/api/Pokemon/{id}/rating` | Get average review rating |
| POST | `/api/Pokemon` | Create a Pokémon with owner and category IDs |
| PUT | `/api/Pokemon` | Update a Pokémon using its ID in the body |
| DELETE | `/api/Pokemon/{id}` | Delete a Pokémon |

Additional controllers expose categories, countries, owners, reviews, and reviewers. Swagger lists the available operations.

An existing Pokémon without reviews has rating `0`; an unknown Pokémon returns `404` from the rating endpoint.

Example create body (use existing owner and category IDs):

```json
{
  "name": "Eevee",
  "birthDate": "2000-01-01T00:00:00",
  "ownerId": 1,
  "categoryId": 1
}
```

## Tests

```bash
dotnet test tests/PokemonWebAPI.Tests/PokemonWebAPI.Tests.csproj --configuration Release
```

Tests cover empty reviews, fractional and single-review averages, separation between Pokémon, and controller action results for existing and missing IDs. They use SQLite and do not require SQL Server. These are repository and controller tests, not full HTTP integration tests. GitHub Actions runs the same command.

## Current limitations

This is a learning project. Authentication, pagination, consistent error responses, and complete input validation remain improvements to make. Repository write methods currently suppress database exceptions, and owner/category validation needs improvement. Tests currently focus on the rating behavior.

## Structure

```text
YouTubeWebAPI/
  Controllers/  HTTP endpoints
  DTOs/         Transfer objects
  Data/         EF Core context
  Models/       Database entities
  Interface/    Repository contracts
  Repository/   Data access
  Migrations/   SQL Server schema history
tests/          Rating regression tests
```
