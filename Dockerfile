#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 443
ENV ASPNETCORE_URLS=""
ENV ASPNETCORE_HTTPS_PORT=""
ENV PGSQLCONNSTR_IdentityDb=""
ENV ALLOWED_ORIGINS=""
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/certs/fullchain.pem"
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath="/certs/privkey.pem"

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["CryptoStashIdentity.csproj", "."]
RUN dotnet restore "./CryptoStashIdentity.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CryptoStashIdentity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoStashIdentity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoStashIdentity.dll"]