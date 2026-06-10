# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy csproj files
COPY Order.API/Order.API.csproj Order.API/
COPY Order.API.Tests/Order.API.Tests.csproj Order.API.Tests/

RUN dotnet restore Order.API/Order.API.csproj

COPY . .

# Publish
RUN dotnet publish Order.API/Order.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Non-root user
RUN useradd -m appuser

# Copy app
COPY --from=build /app/publish .

RUN chown -R appuser:appuser /app

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Order.API.dll"]