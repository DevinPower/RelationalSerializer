FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN mkdir -p /etc/rsconfig/
WORKDIR /etc/rsconfig/

COPY ContainerSetup /etc/rsconfig/
RUN ls -l /etc/rsconfig/

ENTRYPOINT ["./entrypoint.sh"]