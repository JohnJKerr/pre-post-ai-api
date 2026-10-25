# Builds the API into a runtime image that listens on port 8080 as a non-root user.
# Configuration comes from the environment (Mongo__ConnectionString, GitHub__ClientSecret, ...).

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Restore first, from the project files alone, so the layer survives source-only changes.
COPY src/Api/Api.csproj src/Api/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/External/External.csproj src/External/
COPY src/Storage/Storage.csproj src/Storage/
RUN dotnet restore src/Api/Api.csproj

COPY src/ src/
RUN dotnet publish src/Api/Api.csproj --no-restore --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]
