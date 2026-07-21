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
| `Azure__Enabled` | `true` | Enable Azure-backed services |
| `UseAzureServices` | `true` | Enable Azure-backed services |
| `AzureStorage__ConnectionString` | `DefaultEndpointsProtocol=https;...` | Resume file storage |
| `AzureStorage__ContainerName` | `uploads` | Resume blob container |
| `AI__Provider` | `azure` or `openai` | AI provider selector |
| `AI__Azure__Endpoint` | `https://...openai.azure.com` | Azure OpenAI endpoint |
| `AI__Azure__ApiKey` | `<azure-openai-key>` | Azure OpenAI key |
| `AI__Azure__Deployment` | `<deployment-name>` | Azure OpenAI deployment/model |
| `AI__OpenAI__ApiKey` | `sk-...` | OpenAI API key if using OpenAI directly |
| `AI__OpenAI__Model` | `<model-name>` | OpenAI model |
| `EmailSettings__Host` | `smtp.gmail.com` | Gmail SMTP host |
| `EmailSettings__Username` | `user@gmail.com` | Gmail account |
| `EmailSettings__Password` | `<gmail-app-password>` | Gmail app password |
| `EmailSettings__FromEmail` | `user@gmail.com` | Sender address |
| `CalendarSettings__Provider` | `Google` | Calendar provider |
| `CalendarSettings__ClientId` | `<google-client-id>` | Google OAuth client |
| `CalendarSettings__ClientSecret` | `<google-client-secret>` | Google OAuth secret |
| `CalendarSettings__RefreshToken` | `<google-refresh-token>` | Google OAuth refresh token |
| `CalendarSettings__CalendarId` | `primary` | Target calendar |

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
| `AZURE_OPENAI_API_KEY` | App Service setting | Azure OpenAI key |
| `OPENAI_API_KEY` | App Service setting | OpenAI API key |
| `AZURE_STORAGE_CONNECTION_STRING` | App Service setting | Blob storage connection |
| `GOOGLE_CALENDAR_CLIENT_SECRET` | App Service setting | Google Calendar OAuth secret |
| `GMAIL_APP_PASSWORD` | App Service setting | Gmail SMTP app password |
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
Azure__Enabled=true
UseAzureServices=true
AzureStorage__ConnectionString=UseDevelopmentStorage=true
AzureStorage__ContainerName=uploads
AI__Provider=azure
AI__Azure__Endpoint=https://your-resource.openai.azure.com
AI__Azure__ApiKey=your-dev-azure-openai-key
AI__Azure__Deployment=your-deployment-name
EmailSettings__Host=smtp.gmail.com
EmailSettings__Username=your-gmail-address
EmailSettings__Password=your-gmail-app-password
EmailSettings__FromEmail=your-gmail-address
CalendarSettings__Provider=Google
CalendarSettings__ClientId=your-google-client-id
CalendarSettings__ClientSecret=your-google-client-secret
CalendarSettings__RefreshToken=your-google-refresh-token
CalendarSettings__CalendarId=primary

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
