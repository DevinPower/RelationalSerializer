param([string]$InstanceName, [string]$DBPassword, [int]$Port, [string]$NetworkBridge)

docker stop $InstanceName
docker rm $InstanceName

Write-Host "This script will go through creating a Docker image that will house data for your project."

if ("" -eq $InstanceName -or $null -eq $InstanceName){
    $InstanceName = Read-Host "Instance Name"
}

if ("" -eq $DBPassword -or $null -eq $DBPassword){
    $DBPassword = Read-Host "Database (SA) Password"
}

docker pull mcr.microsoft.com/mssql/server:2022-latest

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$DBPassword" `
   -p "$($Port):1433" --name $InstanceName --hostname $InstanceName `
   --net=$NetworkBridge `
   -d `
   mcr.microsoft.com/mssql/server:2022-latest
   
while ($true){
	if ((docker container inspect -f '{{.State.Running}}' $InstanceName) -eq "true"){
		break
	}
	Write-Host "Awaiting container $InstanceName to be running."
	Start-Sleep 10
}

Start-Sleep 10

Get-ChildItem ./DBSetup/SQL | ForEach{
    Write-Host "Executing" $_.Name
    $Content = (Get-Content $_ -Raw).Replace("\n", "`\n")

    docker exec -it $InstanceName /opt/mssql-tools/bin/sqlcmd `
        -S localhost -U SA -P "$DBPASSWORD" `
        -Q $Content
}