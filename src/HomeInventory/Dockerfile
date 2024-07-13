#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.303 AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["HomeInventory.Api/HomeInventory.Api.csproj", "HomeInventory.Api/"]
COPY ["HomeInventory.Infrastructure/HomeInventory.Infrastructure.csproj", "HomeInventory.Infrastructure/"]
COPY ["HomeInventory.Application/HomeInventory.Application.csproj", "HomeInventory.Application/"]
COPY ["HomeInventory.Domain/HomeInventory.Domain.csproj", "HomeInventory.Domain/"]
COPY ["HomeInventory.Domain.Primitives/HomeInventory.Domain.Primitives.csproj", "HomeInventory.Domain.Primitives/"]
COPY ["HomeInventory.Web/HomeInventory.Web.csproj", "HomeInventory.Web/"]
COPY ["HomeInventory.Contracts.Validations/HomeInventory.Contracts.Validations.csproj", "HomeInventory.Contracts.Validations/"]
COPY ["HomeInventory.Contracts/HomeInventory.Contracts.csproj", "HomeInventory.Contracts/"]
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
