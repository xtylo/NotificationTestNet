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