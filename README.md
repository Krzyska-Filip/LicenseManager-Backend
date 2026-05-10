# LicenseManager

## Struktura projektu

```
/
├── Api/                  # Warstwa HTTP (kontrolery, filtry, żądania)
│   ├── Controllers/      # OData controllers (Users, Groups, Licenses, Seats)
│   ├── Filters/          # Swashbuckle document filters
│   └── Requests/         # Request modele (NewXRequest)
├── Database/             # Warstwa danych
│   ├── Entities/         # Encje EF Core z konfiguracją (IEntityTypeConfiguration)
│   ├── Migrations/       # Migracje EF Core
│   └── Seeders/V1/       # Dane testowe (10 użytkowników, grupy, licencje, stanowiska)
└── docker-compose.yml    # API + PostgreSQL 17
```

## Uruchamianie

```bash
docker-compose up --build
```

API dostępne pod: `http://localhost:8080`

Połączenie z bazą konfigurowane przez zmienną środowiskową:

```
ConnectionStrings__DefaultConnection=Host=database-local;Port=5432;Database=licenses;Username=licenses;Password=changeme
```

## Migracje

```bash
dotnet ef migrations add <NazwaMigracji> --project Database --startup-project Api
dotnet ef database update --project Database --startup-project Api
```
