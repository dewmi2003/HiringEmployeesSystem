# Deployment Guide — AI Recruitment Platform

## Prerequisites

1. **Azure Subscription** with Owner/Contributor access
2. **Azure CLI** installed (`az --version`)
3. **.NET 8 SDK** (`dotnet --version`)
4. **GitHub** repository with Actions enabled

---

## Step 1 — Create Azure Resource Group

```bash
az group create \
  --name rg-recruitment-platform \
  --location eastus
```

---

## Step 2 — Create Azure SQL Database

```bash
# Create SQL Server
az sql server create \
  --resource-group rg-recruitment-platform \
  --name sql-recruitment-server \
  --admin-user recruitmentsqladmin \
  --admin-password "<STRONG_PASSWORD>" \
  --location eastus

# Allow Azure services
az sql server firewall-rule create \
  --resource-group rg-recruitment-platform \
  --server sql-recruitment-server \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Create database
az sql db create \
  --resource-group rg-recruitment-platform \
  --server sql-recruitment-server \
  --name RecruitmentDB \
  --service-objective S1
```

---

## Step 3 — Create Azure App Service

```bash
# App Service Plan
az appservice plan create \
  --resource-group rg-recruitment-platform \
  --name plan-recruitment \
  --sku B2 \
  --is-linux

# Web App
az webapp create \
  --resource-group rg-recruitment-platform \
  --plan plan-recruitment \
  --name app-recruitment-api \
  --runtime "DOTNETCORE:8.0"
```

---

## Step 4 — Configure App Settings

```bash
az webapp config appsettings set \
  --resource-group rg-recruitment-platform \
  --name app-recruitment-api \
  --settings \
    "ConnectionStrings__DefaultConnection=<SQL_CONNECTION_STRING>" \
    "Jwt__Key=<JWT_SECRET>" \
    "Jwt__Issuer=RecruitmentAPI" \
    "Jwt__Audience=RecruitmentClient" \
    "Azure__Enabled=true" \
    "UseAzureServices=true" \
    "AzureStorage__ConnectionString=<BLOB_CONNECTION_STRING>" \
    "AzureStorage__ContainerName=uploads" \
    "AI__Provider=azure" \
    "AI__Azure__Endpoint=<AZURE_OPENAI_ENDPOINT>" \
    "AI__Azure__ApiKey=<AZURE_OPENAI_API_KEY>" \
    "AI__Azure__Deployment=<AZURE_OPENAI_DEPLOYMENT>" \
    "AI__Azure__ApiVersion=2024-10-21" \
    "EmailSettings__Host=smtp.gmail.com" \
    "EmailSettings__Port=587" \
    "EmailSettings__Username=<GMAIL_ADDRESS>" \
    "EmailSettings__Password=<GMAIL_APP_PASSWORD>" \
    "EmailSettings__FromEmail=<GMAIL_ADDRESS>" \
    "CalendarSettings__Provider=Google" \
    "CalendarSettings__ClientId=<GOOGLE_CLIENT_ID>" \
    "CalendarSettings__ClientSecret=<GOOGLE_CLIENT_SECRET>" \
    "CalendarSettings__RefreshToken=<GOOGLE_REFRESH_TOKEN>" \
    "CalendarSettings__CalendarId=primary" \
    "ASPNETCORE_ENVIRONMENT=Production"
```

---

## Step 5 — Create Azure Blob Storage

```bash
az storage account create \
  --resource-group rg-recruitment-platform \
  --name stgrecruitmentstorage \
  --sku Standard_LRS \
  --kind StorageV2

az storage container create \
  --account-name stgrecruitmentstorage \
  --name resumecontainer \
  --public-access off
```

---

## Step 6 — Create Azure Static Web App

```bash
az staticwebapp create \
  --resource-group rg-recruitment-platform \
  --name swa-recruitment-frontend \
  --location eastus2 \
  --source https://github.com/<ORG>/<REPO> \
  --branch main \
  --app-location "Frontend/recruitment-client" \
  --output-location "dist" \
  --login-with-github
```

---

## Step 7 — Run Database Migrations

```bash
cd Backend
dotnet ef database update \
  --project Recruitment.Persistence/Recruitment.Persistence.csproj \
  --startup-project Recruitment.API/Recruitment.API.csproj
```

---

## Step 8 — Configure GitHub Secrets

Go to **GitHub Repo → Settings → Secrets and Variables → Actions** and add:

| Secret Name | Value |
|---|---|
| `AZURE_WEBAPP_NAME` | `app-recruitment-api` |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Download from Azure Portal → App → Get publish profile |
| `AZURE_SQL_CONNECTION_STRING` | Full SQL connection string |
| `AZURE_STATIC_WEBAPP_TOKEN` | Token from Step 6 output |
| `JWT_SECRET` | Random 64-char string |
| `AZURE_OPENAI_API_KEY` | Azure OpenAI key |
| `OPENAI_API_KEY` | OpenAI API key if using OpenAI directly |
| `AZURE_STORAGE_CONNECTION_STRING` | Blob Storage connection string |
| `GOOGLE_CALENDAR_CLIENT_SECRET` | Google OAuth secret |
| `GOOGLE_CALENDAR_REFRESH_TOKEN` | Google OAuth refresh token |
| `GMAIL_APP_PASSWORD` | Gmail SMTP app password |
| `VITE_API_BASE_URL` | API URL e.g. `https://app-recruitment-api.azurewebsites.net` |
