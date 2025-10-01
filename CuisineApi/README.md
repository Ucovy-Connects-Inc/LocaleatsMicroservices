CuisineApi - simple .NET 7 Web API with EF Core (SQL Server)

Quick start (PowerShell):

1. Restore and build

   dotnet restore; dotnet build

2. Add EF Core tools (if not installed)

   dotnet tool install --global dotnet-ef

3. Create a migration and update database

   dotnet ef migrations add InitialCreate -p .; dotnet ef database update -p .

4. Run the API

   dotnet run

The API will be available at https://localhost:PORT. Swagger UI is enabled in Development.

Endpoints:
- GET /api/cuisines
- GET /api/cuisines/{id}
- POST /api/cuisines
- PUT /api/cuisines/{id}
- DELETE /api/cuisines/{id}
