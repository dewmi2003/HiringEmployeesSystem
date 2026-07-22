Recruitment.API - Local run instructions

Configuration
- Do not put real passwords in appsettings.json.
- For local private settings, create Backend/Recruitment.API/appsettings.Local.json using appsettings.Local.example.json as the shape.
- For Azure App Service, set these values in Configuration > Application settings / Connection strings.
  - ConnectionStrings:DefaultConnection
  - Jwt__Key, Jwt__Issuer, Jwt__Audience
  - AI__Provider = GitHub Models
  - AI__GitHub__Endpoint = https://models.github.ai/inference
  - AI__GitHub__Token = GitHub personal access token
  - AI__GitHub__Model = openai/gpt-4.1
  - EmailSettings:Host, EmailSettings:Port, EmailSettings:Username, EmailSettings:Password, EmailSettings:FromEmail
  - CalendarSettings:Provider, CalendarSettings:ClientId, CalendarSettings:ClientSecret, CalendarSettings:RefreshToken
  - SeedDemoData = true seeds 100 demo rows for the main backend workflow tables
  - For Azure Blob/SendGrid, set Azure:Enabled=true and corresponding secrets
  - See CLOUD_SETUP.md for the full cloud secret list.

Local run
- From the solution root (where the .sln exists), run the API:
  dotnet run --project Backend/Recruitment.API

AI endpoints
- POST /api/ai/resume-analysis analyzes pasted CV text for ATS score, matched skills, missing skills, keywords, and improvement suggestions.
- GET /api/ai/resumes/{resumeId}/analysis analyzes an uploaded CV already stored in the database. Add ?jobId={jobId} for job-specific matching.
- POST /api/ai/job-match compares CV text with a job description.
- POST /api/ai/candidate-ranking ranks supplied candidate summaries for a job.
- GET /api/ai/jobs/{jobId}/candidate-ranking ranks real applicants for a saved job.
- POST /api/ai/interview-questions generates recruiter interview questions from a job description and candidate summary.

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
