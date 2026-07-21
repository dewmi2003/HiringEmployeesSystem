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

Azure OpenAI
- AI__Provider = azure
- AI__Azure__Endpoint = https://YOUR-RESOURCE-NAME.openai.azure.com
- AI__Azure__ApiKey = Azure OpenAI key
- AI__Azure__Deployment = deployed model/deployment name
- AI__Azure__ApiVersion = 2024-10-21

OpenAI alternative
- AI__Provider = openai
- AI__OpenAI__ApiKey = OpenAI API key
- AI__OpenAI__Model = model name
- AI__OpenAI__Endpoint = https://api.openai.com/v1/chat/completions

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
