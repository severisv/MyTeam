FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY ./dist /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENV TZ="Europe/Oslo"
CMD dotnet server.dll
