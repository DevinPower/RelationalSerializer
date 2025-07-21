param(
    [string]$DBInstanceName = "RelationalSerializerDB", [string]$DBPassword, [int]$DBPort = 8191, 
    [int]$HTTPSAPIPort = 5161, [int]$HTTPAPIPort = 5162, [string]$APIInstanceName = "RelationalSerializerAPI", 
    [string]$NetworkBridge
)

$CurrentBranch = git rev-parse --abbrev-ref HEAD
$CommitCount = git rev-list --count $CurrentBranch
$CurrentVersion = "$CurrentBranch.$CommitCount"

if ("" -eq $DBPassword -or $null -eq $DBPassword){
    $DBPassword = Read-Host "Database (SA) Password"
}

Write-Host "Setting up database"
.\DBSetup\Setup.ps1 -InstanceName $DBInstanceName -DBPassword $DBPassword -Port $DBPort -NetworkBridge $NetworkBridge

Write-Host "Setting up API (Version $CurrentVersion)"

$APISettings = (Get-Content .\DBSetup\Config\appsettings.json -Raw) | ConvertFrom-Json
$APISettings.ConnectionString = "Data Source=$($DBInstanceName),1433;Initial Catalog=RelationalSerializer;User Id=sa;Password=$DBPassword;TrustServerCertificate=true;Encrypt=False"

Set-Content -Path .\DBSetup\Config\appsettings.json -Value ($APISettings | ConvertTo-Json)

Start-Sleep 5

$CurrentImage = (docker inspect --type=image rsapi:$CurrentVersion --format=json | ConvertFrom-JSON)

if ($CurrentImage.count -eq 0){
	docker build -t rsapi:$CurrentVersion .
}

docker run -d -it -p "$($HTTPSAPIPort):443" -p "$($HTTPAPIPort):80" --name $APIInstanceName --net $NetworkBridge rsapi:$CurrentVersion
$AllContent | docker exec -i $APIInstanceName sh -c 'cat > mssql-docker/linux/preview/examples/mssql-customize/setup.sql'

Write-Host "API Listening on https://localhost:$($HTTPSAPIPort) and http://localhost:$($HTTPAPIPort)"