FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GestionOrdinateurs.API/GestionOrdinateurs.API.csproj", "GestionOrdinateurs.API/"]
COPY ["GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj", "GestionOrdinateurs.Shared/"]
RUN dotnet restore "GestionOrdinateurs.API/GestionOrdinateurs.API.csproj"

COPY GestionOrdinateurs.API/ GestionOrdinateurs.API/
COPY GestionOrdinateurs.Shared/ GestionOrdinateurs.Shared/
RUN dotnet publish "GestionOrdinateurs.API/GestionOrdinateurs.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=5015
EXPOSE 5015
ENTRYPOINT ["dotnet", "GestionOrdinateurs.API.dll"]
