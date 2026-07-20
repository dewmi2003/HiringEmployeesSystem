Recruitment.API - Local run instructions

Configuration
- Update Backend/Recruitment.API/appsettings.json or set environment variables for connection string and JWT secrets.
  - ConnectionStrings:DefaultConnection
  - Jwt__Key, Jwt__Issuer, Jwt__Audience
  - AI:Provider = "local" or "azure"
  - For Azure AI, set AI:Azure:ApiKey and AI:Azure:Endpoint
  - For Azure Blob/SendGrid, set Azure:Enabled=true and corresponding secrets

Local run
- From the solution root (where the .sln exists), run the API:
  dotnet run --project Backend/Recruitment.API

Migrations
- From Backend/Recruitment.Persistence project folder:
  dotnet ef migrations add InitialCreate --startup-project ../Recruitment.API
  dotnet ef database update --startup-project ../Recruitment.API

Docker
- A Dockerfile exists at Backend/Recruitment.API/Dockerfile. Build with:
  docker build -t recruitment-api:local Backend/Recruitment.API

Testing
- Run unit tests with dotnet test
