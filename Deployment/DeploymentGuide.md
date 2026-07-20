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
    "AzureBlob__ConnectionString=<BLOB_CONNECTION_STRING>" \
    "AI_API_KEY=<AI_API_KEY>" \
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
| `AI_API_KEY` | Your AI provider API key |
| `BLOB_CONNECTION_STRING` | Blob Storage connection string |
| `VITE_API_BASE_URL` | API URL e.g. `https://app-recruitment-api.azurewebsites.net` |
