Cloud setup checklist

Set these as environment variables, GitHub Actions secrets, or Azure App Service application settings. Do not commit real values to Git.

Database
- ConnectionStrings__DefaultConnection = Azure SQL connection string

JWT
- Jwt__Key = long production signing key
- Jwt__Issuer = RecruitmentAPI
- Jwt__Audience = RecruitmentClient

Azure Blob Storage for CV uploads
- Azure__Enabled = true
- UseAzureServices = true
- AzureStorage__ConnectionString = Azure Storage connection string
- AzureStorage__ContainerName = uploads

GitHub Models
- AI__Provider = GitHub Models
- AI__GitHub__Endpoint = https://models.github.ai/inference
- AI__GitHub__Token = GitHub personal access token with models access
- AI__GitHub__Model = openai/gpt-4.1

Gmail SMTP
- EmailSettings__Host = smtp.gmail.com
- EmailSettings__Port = 587
- EmailSettings__Username = Gmail address
- EmailSettings__Password = Gmail app password
- EmailSettings__FromEmail = Gmail address

Google Calendar
- CalendarSettings__Provider = Google
- CalendarSettings__ClientId = Google OAuth client ID
- CalendarSettings__ClientSecret = Google OAuth client secret
- CalendarSettings__RefreshToken = Google OAuth refresh token
- CalendarSettings__CalendarId = primary
- CalendarSettings__TimeZone = Asia/Colombo

Demo data
- SeedDemoData = true for local/demo databases
- SeedDemoData = false for clean production databases

Built-in demo logins after seeding
- Admin: admin@local / P@ssw0rd!
- Candidate: demo.candidate001@local / P@ssw0rd!
- Recruiter: demo.recruiter001@local / P@ssw0rd!
