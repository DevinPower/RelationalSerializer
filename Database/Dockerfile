FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN mkdir -p /etc/rsconfig/
WORKDIR /etc/rsconfig/

COPY ContainerSetup /etc/rsconfig/
RUN chmod +x /etc/rsconfig/entrypoint.sh
RUN chmod +x /etc/rsconfig/configure-db.sh

ENTRYPOINT ["/etc/rsconfig/entrypoint.sh"]