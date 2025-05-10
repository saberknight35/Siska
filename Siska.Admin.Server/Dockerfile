# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7777

# This stage is used to build the service project
COPY ./Siska.Admin.Server/publish .

ENTRYPOINT ["dotnet", "Siska.Admin.Server.dll"]