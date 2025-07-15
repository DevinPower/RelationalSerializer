docker build -t rsdb:test .
$SAPASS = "Don7L053ThIs"
docker run -e "SA_PASSWORD=$SAPASS" -e "MSSQL_SA_PASSWORD=$SAPASS" -e "ACCEPT_EULA=Y" -p 1477:1433 -d rsdb:test