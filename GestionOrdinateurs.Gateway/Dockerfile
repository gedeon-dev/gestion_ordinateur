FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj", "GestionOrdinateurs.Gateway/"]
COPY ["GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj", "GestionOrdinateurs.Shared/"]
RUN dotnet restore "GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj"

COPY GestionOrdinateurs.Gateway/ GestionOrdinateurs.Gateway/
COPY GestionOrdinateurs.Shared/ GestionOrdinateurs.Shared/
RUN dotnet publish "GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=5010
EXPOSE 5010
ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
