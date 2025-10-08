FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY ./dist /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV TZ="Europe/Oslo"
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
CMD dotnet server.dll
