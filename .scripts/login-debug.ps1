$base='http://localhost:8080'
$login = @{ Username = 'copilot_user'; Password = 'Password1!' } | ConvertTo-Json
$resp = Invoke-RestMethod -Uri "$base/api/authenticate/login" -Method Post -Body $login -ContentType 'application/json' -TimeoutSec 10
Write-Output 'LOGIN RESPONSE:'
$resp | Format-List *
$token = $resp.token
Write-Output 'TOKEN:'
Write-Output $token
