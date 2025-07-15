#!/bin/bash

# Start the script to create the DB and user
/tmp/setup/configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr --accept-eula