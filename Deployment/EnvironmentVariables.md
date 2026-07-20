# Environment Variables — AI Recruitment Platform

## ⚠️ IMPORTANT: Never commit secrets to source control.

All secrets must be stored in **GitHub Secrets** (for CI/CD) and **Azure App Service Application Settings** (for runtime).

---

## Backend — Azure App Service Settings

| Variable | Example | Purpose |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | .NET runtime environment |
| `ConnectionStrings__DefaultConnection` | `Server=...;Database=RecruitmentDB;...` | Azure SQL connection string |
| `Jwt__Key` | `<64-char-random-string>` | JWT signing key (keep secret) |
| `Jwt__Issuer` | `RecruitmentAPI` | JWT token issuer |
| `Jwt__Audience` | `RecruitmentClient` | JWT token audience |
| `AzureBlob__ConnectionString` | `DefaultEndpointsProtocol=https;...` | Resume file storage |
| `AI_API_KEY` | `sk-...` | External AI/ML service key |

---

## Frontend — Azure Static Web App (Build-time)

| Variable | Example | Purpose |
|---|---|---|
| `VITE_API_BASE_URL` | `https://app-recruitment-api.azurewebsites.net` | API endpoint for React app |

---

## GitHub Actions Secrets

| Secret | Used In | Purpose |
|---|---|---|
| `AZURE_WEBAPP_NAME` | Backend workflow | Name of Azure App Service |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Backend workflow | Azure publish profile XML |
| `AZURE_STATIC_WEBAPP_TOKEN` | Frontend workflow | Static Web App deployment token |
| `AZURE_SQL_CONNECTION_STRING` | DB workflow | SQL connection for migrations |
| `JWT_SECRET` | Backend workflow | JWT signing key |
| `AI_API_KEY` | Backend workflow | AI service key |
| `BLOB_CONNECTION_STRING` | Backend workflow | Blob storage connection |
| `VITE_API_BASE_URL` | Frontend workflow | React API base URL |

---

## Local Development (.env)

> **Note:** Create a `.env` file locally. Add `.env` to `.gitignore`.

```env
# Backend (used in launchSettings.json or appsettings.Development.json)
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=RecruitmentDB;User Id=sa;Password=YourLocalPassword;TrustServerCertificate=True;
Jwt__Key=your-local-jwt-secret-min-32-characters
Jwt__Issuer=RecruitmentAPI
Jwt__Audience=RecruitmentClient
AzureBlob__ConnectionString=UseDevelopmentStorage=true
AI_API_KEY=your-dev-api-key

# Frontend (.env.local inside Frontend/recruitment-client/)
VITE_API_BASE_URL=http://localhost:8080
```

---

## Generating a Secure JWT Key

```bash
# PowerShell
[System.Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(64))

# Linux/macOS
openssl rand -base64 64
```
