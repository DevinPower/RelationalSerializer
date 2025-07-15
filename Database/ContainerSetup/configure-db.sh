#!/bin/bash

# Wait 60 seconds for SQL Server to start up by ensuring that 
# calling SQLCMD does not return an error code, which will ensure that sqlcmd is accessible
# and that system and user databases return "0" which means all databases are in an "online" state
# https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql?view=sql-server-2017 

DBSTATUS=1
ERRCODE=1
i=0

while [[ "$DBSTATUS" -ne 0 && "$i" -lt 600 && "$ERRCODE" -ne 0 ]]; do
    ((i++))

    # Run the SQL query and capture both output and error
    OUTPUT=$(/opt/mssql-tools18/bin/sqlcmd -h -1 -t 1 -U sa -P "$SA_PASSWORD" -Q "SET NOCOUNT ON; SELECT SUM(state) FROM sys.databases" 2>&1)
    ERRCODE=$?

    # Trim whitespace and set DBSTATUS only if command succeeded
    if [ "$ERRCODE" -eq 0 ]; then
        DBSTATUS=$(echo "$OUTPUT" | tr -d '[:space:]')
    fi

    echo "Attempt $i: DBSTATUS=$DBSTATUS, ERRCODE=$ERRCODE"
    sleep 1
done

# ❌ FIXED BAD IF SYNTAX: `OR` is not valid in `[ ... ]`
if [[ "$DBSTATUS" -ne 0 || "$ERRCODE" -ne 0 ]]; then 
    echo "❌ SQL Server failed to come online in time or one or more databases are not ONLINE"
    exit 1
fi

for file in /tmp/setup/SQL/*; do
    echo "Running $file"
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i $file
done
