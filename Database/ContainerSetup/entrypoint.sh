#!/bin/bash

# Start the script to create the DB and user
/etc/rsconfig/configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr --accept-eula