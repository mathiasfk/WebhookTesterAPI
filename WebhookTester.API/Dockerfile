# Use the .NET SDK base image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["WebhookTester.API/WebhookTester.API.csproj", "WebhookTester.API/"]
COPY ["WebhookTester.Infrastructure/WebhookTester.Infrastructure.csproj", "WebhookTester.Infrastructure/"]
COPY ["WebhookTester.Core/WebhookTester.Core.csproj", "WebhookTester.Core/"]
RUN dotnet restore "WebhookTester.API/WebhookTester.API.csproj"

# Copy the remaining files and build the application
COPY . .
WORKDIR "/src/WebhookTester.API"
RUN dotnet build "WebhookTester.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "WebhookTester.API.csproj" -c Release -o /app/publish

# Use the .NET Runtime base image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebhookTester.API.dll"]
