# ============================================================
# File: Backend/Recruitment.API/Dockerfile
# Purpose: Multi-stage production Docker image for ASP.NET Core 8 API
# ============================================================

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore (layer caching)
COPY ["Recruitment.API/Recruitment.API.csproj",           "Recruitment.API/"]
COPY ["Recruitment.Application/Recruitment.Application.csproj", "Recruitment.Application/"]
COPY ["Recruitment.Persistence/Recruitment.Persistence.csproj",  "Recruitment.Persistence/"]
COPY ["Recruitment.Domain/Recruitment.Domain.csproj",     "Recruitment.Domain/"]

RUN dotnet restore "Recruitment.API/Recruitment.API.csproj"

# Copy all source files
COPY . .

# Build & publish release
WORKDIR "/src/Recruitment.API"
RUN dotnet publish "Recruitment.API.csproj" -c Release -o /app/publish --no-restore

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Security: run as non-root user
RUN adduser --disabled-password --no-create-home appuser
USER appuser

COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Recruitment.API.dll"]
