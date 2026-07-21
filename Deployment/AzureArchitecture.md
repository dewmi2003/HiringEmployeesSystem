# Azure Architecture — AI Recruitment Platform

## Overview

```
User (Browser / Mobile)
         │
         ▼
Azure Static Web Apps
      (React + TypeScript Frontend)
         │
         │  HTTPS REST API
         ▼
Azure App Service (Linux)
      (ASP.NET Core 8 Web API)
         │
    ┌────┴───────────┐
    ▼                ▼
Azure SQL Database   Azure Blob Storage
(RecruitmentDB)      (Resume Files + AI Docs)
    └───────┬────────┘
            ▼
Azure Application Insights
      (Monitoring & Telemetry)
```

---

## Resource Group

| Resource | Name |
|---|---|
| Resource Group | `rg-recruitment-platform` |
| Region | East US (recommended) |

---

## Azure Resources

| Service | Resource Name | Purpose |
|---|---|---|
| Azure App Service | `app-recruitment-api` | Hosts ASP.NET Core 8 Web API |
| Azure App Service Plan | `plan-recruitment` | Compute plan (B2 recommended) |
| Azure Static Web Apps | `swa-recruitment-frontend` | Hosts React TypeScript frontend |
| Azure SQL Server | `sql-recruitment-server` | SQL Server instance |
| Azure SQL Database | `RecruitmentDB` | Relational database |
| Azure Blob Storage | `stgrecruitmentstorage` | Resume files + AI outputs |
| Azure Application Insights | `appi-recruitment` | Telemetry & monitoring |
| Azure Key Vault | `kv-recruitment` | Secrets management |

---

## App Service — Environment Variables

| Variable | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | Azure SQL connection string |
| `Jwt__Key` | JWT signing secret |
| `Jwt__Issuer` | JWT issuer identifier |
| `Jwt__Audience` | JWT audience |
| `Azure__Enabled` | Enable Azure-backed services |
| `UseAzureServices` | Enable Azure-backed services |
| `AzureStorage__ConnectionString` | Blob Storage connection string |
| `AzureStorage__ContainerName` | Blob container name |
| `AI__Provider` | `azure` or `openai` |
| `AI__Azure__Endpoint` | Azure OpenAI endpoint |
| `AI__Azure__ApiKey` | Azure OpenAI key |
| `AI__Azure__Deployment` | Azure OpenAI deployment/model |
| `AI__OpenAI__ApiKey` | OpenAI key if using OpenAI directly |
| `EmailSettings__Password` | Gmail app password |
| `CalendarSettings__RefreshToken` | Google Calendar OAuth refresh token |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

---

## Security

- HTTPS enforced at App Service level
- JWT Bearer authentication on all protected API endpoints
- Azure SQL firewall: allow Azure services + specific IP ranges only
- Managed Identity for Blob and Key Vault access (no connection strings in code)
- CORS: configured to only allow the Static Web App domain
- Rate limiting: configured via ASP.NET Core middleware
