Recruitment.API - Local run instructions

Configuration
- Update Backend/Recruitment.API/appsettings.json or set environment variables for connection string and JWT secrets.
  - ConnectionStrings:DefaultConnection
  - Jwt__Key, Jwt__Issuer, Jwt__Audience
  - AI:Provider = "azure" or "openai"
  - For Azure AI, set AI:Azure:Endpoint, AI:Azure:ApiKey, and AI:Azure:Deployment
  - For OpenAI, set AI:OpenAI:ApiKey and AI:OpenAI:Model
  - EmailSettings:Host, EmailSettings:Port, EmailSettings:Username, EmailSettings:Password, EmailSettings:FromEmail
  - CalendarSettings:Provider, CalendarSettings:ClientId, CalendarSettings:ClientSecret, CalendarSettings:RefreshToken
  - SeedDemoData = true seeds 100 demo rows for the main backend workflow tables
  - For Azure Blob/SendGrid, set Azure:Enabled=true and corresponding secrets
  - See CLOUD_SETUP.md for the full cloud secret list.

Local run
- From the solution root (where the .sln exists), run the API:
  dotnet run --project Backend/Recruitment.API

Demo logins
- Admin: admin@local / P@ssw0rd!
- Candidate: demo.candidate001@local / P@ssw0rd!
- Recruiter: demo.recruiter001@local / P@ssw0rd!

Migrations
- From Backend/Recruitment.Persistence project folder:
  dotnet ef migrations add InitialCreate --startup-project ../Recruitment.API
  dotnet ef database update --startup-project ../Recruitment.API

Docker
- A Dockerfile exists at Backend/Recruitment.API/Dockerfile. Build with:
  docker build -t recruitment-api:local Backend/Recruitment.API

Testing
- Run unit tests with dotnet test
