#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY app/publish/ .

#ENTRYPOINT ["dotnet", "StadtLandFussApi.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet StadtLandFussApi.dll
