docker build -t rsdb:test .
$SAPASS = "Don7L053ThIs"
docker run -p 1477:1433 rsdb:test -e "SA_PASSWORD=$SAPASS" -e "MSSQL_SA_PASSWORD=$SAPASS" =e "ACCEPT_EULA=Y"