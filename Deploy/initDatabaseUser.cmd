@IF [%1] == [] goto :argsError

@SET CurrentFolder=%~dp0

sqlcmd -S %1 -i %CurrentFolder%InitDatabaseUser.sql

@goto :eof

:argsError
echo Please input SQL Server instance name: [protocol:]server[instance_name][,port]
echo Example: .\sqlexpress
exit 1