$client = New-Object System.Net.Http.HttpClient
$resp = $client.GetAsync('http://localhost:8080/api/v1/ingredients').Result
Write-Output $resp.StatusCode
$body = $resp.Content.ReadAsStringAsync().Result
if ($body.Length -gt 400) { $body = $body.Substring(0,400) + '...'}
Write-Output $body
