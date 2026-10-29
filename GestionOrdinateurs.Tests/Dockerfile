FROM mcr.microsoft.com/dotnet/sdk:10.0 AS test
WORKDIR /src

COPY ["GestionOrdinateurs.Tests/GestionOrdinateurs.Tests.csproj", "GestionOrdinateurs.Tests/"]
COPY ["GestionOrdinateurs.API/GestionOrdinateurs.API.csproj", "GestionOrdinateurs.API/"]
COPY ["GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj", "GestionOrdinateurs.Shared/"]
RUN dotnet restore "GestionOrdinateurs.Tests/GestionOrdinateurs.Tests.csproj"

COPY GestionOrdinateurs.Tests/ GestionOrdinateurs.Tests/
COPY GestionOrdinateurs.API/ GestionOrdinateurs.API/
COPY GestionOrdinateurs.Shared/ GestionOrdinateurs.Shared/
ENTRYPOINT ["dotnet", "test", "GestionOrdinateurs.Tests/GestionOrdinateurs.Tests.csproj", "--no-restore", "--configuration", "Release"]
