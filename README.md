# WebhookTesterAPI

Generate HTTP webhooks and monitor in real-time what's sent to them.

## Setup
To set the database up:
```ps1
cd WebhookTester.Infrastructure/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Updating DB schema
To apply changes on the Core entities to the database:
```ps1
cd WebhookTester.Infrastructure/
dotnet ef migrations add MigrationName
dotnet ef database update
```