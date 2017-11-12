docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=kind6iVy' \
   --name 'sql1' -p 1401:1433 \
   -v /home/igor/data/mssql:/var/opt/mssql \
   -d microsoft/mssql-server-linux:2017-latest
