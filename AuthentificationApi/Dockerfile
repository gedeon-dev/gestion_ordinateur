FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AuthentificationApi/AuthentificationApi.csproj", "AuthentificationApi/"]
COPY ["GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj", "GestionOrdinateurs.Shared/"]
RUN dotnet restore "AuthentificationApi/AuthentificationApi.csproj"

COPY AuthentificationApi/ AuthentificationApi/
COPY GestionOrdinateurs.Shared/ GestionOrdinateurs.Shared/
RUN dotnet publish "AuthentificationApi/AuthentificationApi.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=5273
EXPOSE 5273
ENTRYPOINT ["dotnet", "AuthentificationApi.dll"]
