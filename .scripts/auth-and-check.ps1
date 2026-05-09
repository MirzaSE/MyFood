$base = 'http://localhost:8080'
$reg = @{ Username = 'copilot_user'; Password = 'Password1!' } | ConvertTo-Json
try {
  Invoke-RestMethod -Uri "$base/api/authenticate/register" -Method Post -Body $reg -ContentType 'application/json' -TimeoutSec 10 | Out-Null
} catch { }
$login = @{ Username = 'copilot_user'; Password = 'Password1!' } | ConvertTo-Json
$resp = Invoke-RestMethod -Uri "$base/api/authenticate/login" -Method Post -Body $login -ContentType 'application/json' -TimeoutSec 10
$token = $resp.token -or $resp.Token -or $resp.accessToken -or $resp.AccessToken
if (-not $token) { Write-Output 'NO_TOKEN'; exit 1 }
$headers = @{ Authorization = "Bearer $token" }
$ingredients = Invoke-RestMethod -Uri "$base/api/v1/ingredients" -Method Get -Headers $headers -TimeoutSec 10
Write-Output 'STATUS: 200'
$ingredients | ConvertTo-Json -Depth 4 | Select-Object -First 1
