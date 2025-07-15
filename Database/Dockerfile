FROM mcr.microsoft.com/mssql/server:2022-latest
RUN mkdir -p /tmp/setup
WORKDIR /tmp/setup

COPY /ContainerSetup /tmp/setup

ENTRYPOINT ["./entrypoint.sh"]