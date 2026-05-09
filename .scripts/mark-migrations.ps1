$cs = "Server=localhost;Database=MyFood_Database;User Id=SA;Password=Password1!;TrustServerCertificate=True"
$con = New-Object System.Data.SqlClient.SqlConnection $cs
$con.Open()
$migrations = @(
    '20250312150424_Initial',
    '20250312162119_ReduceCharacters',
    '20260325112226_AuthenticationUser'
)
foreach ($m in $migrations) {
    $cmd = $con.CreateCommand()
    $cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId='$m') INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES('$m','10.0.5')"
    $cmd.ExecuteNonQuery() | Out-Null
}
$con.Close()
Write-Output 'DONE'