try {
  $wc = New-Object System.Net.WebClient
  $body = $wc.DownloadString('http://localhost:8080/api/v1/ingredients')
  Write-Output 'OK'
  if ($body.Length -gt 400) { $s = $body.Substring(0,400) + '...'} else { $s = $body }
  Write-Output $s
} catch {
  Write-Output 'ERROR'
  Write-Output $_.Exception.Message
}