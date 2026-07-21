$ProgressPreference = 'SilentlyContinue'
$baseUrl = "http://localhost:5000"

# 1. Register a new Candidate
Write-Host "1. Registering candidate..."
$regBody = @{
    email    = "candidate_test@local.com"
    password = "Password123!"
    fullName = "Test Candidate"
    role     = "Candidate"
} | ConvertTo-Json

try {
    $regResp = Invoke-RestMethod -Uri "$baseUrl/api/Auth/register" -Method Post -Body $regBody -ContentType "application/json"
    $token = $regResp.token
    Write-Host "Registered successfully. Token acquired."
}
catch {
    Write-Host "Registration failed or already registered, attempting login..."
}

# 2. Login to get token
$loginBody = @{
    email    = "candidate_test@local.com"
    password = "Password123!"
} | ConvertTo-Json

$loginResp = Invoke-RestMethod -Uri "$baseUrl/api/Auth/login" -Method Post -Body $loginBody -ContentType "application/json"
$token = $loginResp.token
Write-Host "Logged in. Token: $($token.Substring(0, 20))...."

$headers = @{
    Authorization = "Bearer $token"
}

# Get Candidate profile to retrieve CandidateId
$profile = Invoke-RestMethod -Uri "$baseUrl/api/Candidates/me" -Method Get -Headers $headers
$candidateId = $profile.candidateId
Write-Host "Candidate ID: $candidateId"

# Create a temporary resume text file
$resumePathV1 = New-TemporaryFile
Set-Content -Path $resumePathV1 -Value "My Resume Version 1. I have skills in ASP.NET Core, EF Core, and SQL." -Encoding Utf8

Write-Host "3. Uploading Resume Version 1..."
$LF = "`r`n"
$boundary = [System.Guid]::NewGuid().ToString()
$contentType = "multipart/form-data; boundary=$boundary"

# Build body
$bodyLines = (
    "--$boundary",
    "Content-Disposition: form-data; name=`"file`"; filename=`"resume_v1.txt`"",
    "Content-Type: text/plain",
    "",
    (Get-Content $resumePathV1 -Raw),
    "--$boundary--",
    ""
) -join $LF

$uploadResp = Invoke-RestMethod -Uri "$baseUrl/api/resumes/upload" -Method Post -Headers $headers -Body $bodyLines -ContentType $contentType
$resumeId = $uploadResp.id
Write-Host "Uploaded Resume. Version: $($uploadResp.version), IsActive: $($uploadResp.isActive), ResumeId: $resumeId"

# Upload Version 2 (Updating version 1 by using PUT api/resumes/{id}/update)
$resumePathV2 = New-TemporaryFile
Set-Content -Path $resumePathV2 -Value "My Resume Version 2. Added Azure and Docker skills." -Encoding Utf8

Write-Host "4. Uploading Resume Version 2..."
$bodyLinesV2 = (
    "--$boundary",
    "Content-Disposition: form-data; name=`"file`"; filename=`"resume_v2.txt`"",
    "Content-Type: text/plain",
    "",
    (Get-Content $resumePathV2 -Raw),
    "--$boundary--",
    ""
) -join $LF

$updateResp = Invoke-RestMethod -Uri "$baseUrl/api/resumes/$resumeId/update" -Method Put -Headers $headers -Body $bodyLinesV2 -ContentType $contentType
Write-Host "Updated. Created new version. Id: $($updateResp.id), Version: $($updateResp.version), IsActive: $($updateResp.isActive)"
$newResumeId = $updateResp.id

# Get history
Write-Host "5. Fetching History..."
$history = Invoke-RestMethod -Uri "$baseUrl/api/resumes/candidate/$candidateId/history" -Method Get -Headers $headers
foreach ($h in $history) {
    Write-Host "  - Version: $($h.version), Active: $($h.isActive), File: $($h.fileName)"
}

# Download file
Write-Host "6. Downloading Resume V2..."
$downloadResp = Invoke-RestMethod -Uri "$baseUrl/api/resumes/$newResumeId/download" -Method Get -Headers $headers
Write-Host "Downloaded content: $downloadResp"

# Login as Admin to search
Write-Host "7. Search (as Admin)..."
$adminLoginBody = @{
    email    = "admin@local"
    password = "P@ssw0rd!"
} | ConvertTo-Json
$adminLoginResp = Invoke-RestMethod -Uri "$baseUrl/api/Auth/login" -Method Post -Body $adminLoginBody -ContentType "application/json"
$adminToken = $adminLoginResp.token
$adminHeaders = @{
    Authorization = "Bearer $adminToken"
}

$searchResults = Invoke-RestMethod -Uri "$baseUrl/api/resumes/search?searchTerm=Docker" -Method Get -Headers $adminHeaders
Write-Host "Search Results for 'Docker':"
foreach ($r in $searchResults) {
    Write-Host "  - Resume Id: $($r.id), CandidateId: $($r.candidateId), File: $($r.fileName)"
}

# Soft delete active version
Write-Host "8. Soft Deleting Active Version..."
Invoke-RestMethod -Uri "$baseUrl/api/resumes/$newResumeId/soft" -Method Delete -Headers $headers
Write-Host "Soft deleted successfully."

# Fetch history again
$history2 = Invoke-RestMethod -Uri "$baseUrl/api/resumes/candidate/$candidateId/history" -Method Get -Headers $headers
Write-Host "History after soft delete:"
foreach ($h in $history2) {
    Write-Host "  - Version: $($h.version), Active: $($h.isActive), File: $($h.fileName)"
}

# Clean up temp files
Remove-Item $resumePathV1 -ErrorAction SilentlyContinue
Remove-Item $resumePathV2 -ErrorAction SilentlyContinue
