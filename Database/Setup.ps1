param([string]$InstanceName, [string]$DBPassword, [int]$Port, [string]$NetworkBridge)

docker stop $InstanceName
docker rm $InstanceName

Write-Host "This script will go through creating a Docker image that will house data for your project."

if ("" -eq $InstanceName -or $null -eq $InstanceName){
    $InstanceName = Read-Host "Instance Name"
}

$CurrentDatabase = (docker inspect --type=image mcr.microsoft.com/mssql/server:2022-latest --format=json | ConvertFrom-JSON)

if ($CurrentDatabase.count -eq 0){
	docker pull mcr.microsoft.com/mssql/server:2022-latest
}

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

$AllContent = ""
ForEach($Script in Get-ChildItem ./DBSetup/SQL){
    Write-Host "Executing" $Script.Name
    $AllContent += (Get-Content $Script -Raw).Replace("\n", "`\n") + '`n'
}
#mssql-docker/linux/preview/examples/mssql-customize/setup.sql