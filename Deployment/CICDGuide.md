# CI/CD Guide — AI Recruitment Platform

## Overview

Three GitHub Actions workflows automate build, test, and deployment:

| Workflow | File | Trigger |
|---|---|---|
| Backend CI/CD | `backend-ci-cd.yml` | Push to `main`/`development` under `Backend/**` |
| Frontend CI/CD | `frontend-ci-cd.yml` | Push to `main`/`development` under `Frontend/**` |
| Database Deploy | `database-deploy.yml` | Push to `main` under `Database/**` or manual trigger |

---

## Git Branch Strategy

```
feature/* → development → main → Production Deploy
```

| Branch | Purpose |
|---|---|
| `main` | Production deployments trigger here |
| `development` | Integration branch, staging deploys |
| `feature/backend` | Backend changes |
| `feature/frontend` | Frontend changes |
| `feature/database` | Database schema changes |
| `feature/ai` | AI service changes |

### Workflow

1. Create a `feature/*` branch from `development`
2. Open a Pull Request to `development`
3. PR triggers `build-and-test` job (no deploy)
4. After merge to `development`, staging is updated
5. PR from `development` → `main` triggers full production deploy

---

## Backend Pipeline Details

```yaml
Jobs:
  build-and-test:
    - Checkout → Setup .NET 8 → Restore → Build → Test → Publish

  deploy:
    - Download artifact → Deploy to Azure App Service
    - Runs only on push to main
```

---

## Frontend Pipeline Details

```yaml
Jobs:
  build-and-deploy:
    - Checkout → Setup Node 20 → npm ci → npm run build
    - Deploy to Azure Static Web Apps (only on push to main)
```

---

## Database Pipeline Details

```yaml
Jobs:
  migrate:
    - dotnet ef database update (uses AZURE_SQL_CONNECTION_STRING)

  run-sql-scripts:
    - Apply Indexes.sql
    - Apply StoredProcedures.sql
    - Apply SeedData.sql (first deploy)
```

---

## Required GitHub Secrets

| Secret | Used In |
|---|---|
| `AZURE_WEBAPP_NAME` | `backend-ci-cd.yml` |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | `backend-ci-cd.yml` |
| `AZURE_STATIC_WEBAPP_TOKEN` | `frontend-ci-cd.yml` |
| `AZURE_SQL_CONNECTION_STRING` | `database-deploy.yml` |
| `VITE_API_BASE_URL` | `frontend-ci-cd.yml` |

---

## Running Pipelines Manually

The `database-deploy.yml` supports manual `workflow_dispatch`. To trigger it:

1. Go to **GitHub → Actions → Database Deploy**
2. Click **Run workflow**
3. Select target environment (`production` or `staging`)
