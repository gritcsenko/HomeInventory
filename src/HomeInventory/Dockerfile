#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Build
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src

COPY . .
RUN dotnet restore "./HomeInventory.Api/HomeInventory.Api.csproj" --disable-parallel
RUN dotnet publish "./HomeInventory.Api/HomeInventory.Api.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false

#Serve
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "HomeInventory.Api.dll"]