# LicenseManager

## Project Structure

```
/
├── Api/                  # HTTP layer (controllers, filters, requests)
│   ├── Controllers/      # OData controllers (Users, Groups, Licenses, Seats)
│   ├── Filters/          # Swashbuckle document filters
│   └── Requests/         # Request models (NewXRequest)
├── Database/             # Data layer
│   ├── Entities/         # EF Core entities with configuration (IEntityTypeConfiguration)
│   ├── Migrations/       # EF Core migrations
│   └── Seeders/V1/       # Seed data (10 users, groups, licenses, seats)
└── docker-compose.yml    # API + PostgreSQL 17
```

## Running

```bash
docker-compose up --build
```

API available at: `http://localhost:8080`

Database connection is configured via environment variable:

```
ConnectionStrings__DefaultConnection=Host=database-local;Port=5432;Database=licenses;Username=licenses;Password=changeme
```

## Migrations

```bash
dotnet ef migrations add <MigrationName> --project Database --startup-project Api
dotnet ef database update --project Database --startup-project Api
```
