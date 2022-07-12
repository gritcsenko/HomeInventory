#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HomeInventory.Api/HomeInventory.Api.csproj", "HomeInventory.Api/"]
RUN dotnet restore "HomeInventory.Api/HomeInventory.Api.csproj"
COPY . .
WORKDIR "/src/HomeInventory.Api"
RUN dotnet build "HomeInventory.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeInventory.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeInventory.Api.dll"]