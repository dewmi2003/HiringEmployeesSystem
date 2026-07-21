try {
    $r = Invoke-RestMethod -Uri "http://localhost:5000/api/Auth/login" -Method Post -Body '{"email":"admin@local","password":"P@ssw0rd!"}' -ContentType "application/json"
    $r
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
    $stream = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($stream)
    Write-Host "Response Body: $($reader.ReadToEnd())"
}
