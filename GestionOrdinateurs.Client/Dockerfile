FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj", "GestionOrdinateurs.Client/"]
COPY ["GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj", "GestionOrdinateurs.Shared/"]
RUN dotnet restore "GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj"

COPY GestionOrdinateurs.Client/ GestionOrdinateurs.Client/
COPY GestionOrdinateurs.Shared/ GestionOrdinateurs.Shared/
RUN dotnet publish "GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "GestionOrdinateurs.Client.dll"]
