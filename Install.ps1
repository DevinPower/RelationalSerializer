param(
    [string]$DBInstanceName = "RelationalSerializerDB", [string]$DBPassword, [int]$DBPort = 8191, 
    [int]$HTTPSAPIPort = 5161, [int]$HTTPAPIPort = 5162, [string]$APIInstanceName = "RelationalSerializerAPI", 
    [string]$NetworkBridge
)


Write-Host "Setting up database"
.\DBSetup\Setup.ps1 -InstanceName $DBInstanceName -DBPassword $DBPassword -Port $DBPort -NetworkBridge $NetworkBridge

Write-Host "Setting up API"

$APISettings = (Get-Content .\DBSetup\Config\appsettings.json -Raw) | ConvertFrom-Json
$APISettings.ConnectionString = "Data Source=$($DBInstanceName),1433;Initial Catalog=RelationalSerializer;User Id=sa;Password=$DBPassword;TrustServerCertificate=true;Encrypt=False"

Set-Content -Path .\DBSetup\Config\appsettings.json -Value ($APISettings | ConvertTo-Json)

Start-Sleep 5

docker build -t webapi .
docker run -d -it --rm -p "$($HTTPSAPIPort):443" -p "$($HTTPAPIPort):80" --name $APIInstanceName --net=$NetworkBridge webapi


Write-Host "API Listening on https://localhost:$($HTTPSAPIPort) and http://localhost:$($HTTPAPIPort)"