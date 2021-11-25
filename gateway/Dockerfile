FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY *.sln .
COPY src/Gateway/*.csproj ./src/Gateway/
RUN dotnet restore

# copy full solution over
COPY . .
RUN dotnet build

# publish the service
FROM build as publish
WORKDIR /app/src/Gateway
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=publish /app/src/Gateway/out ./
ENTRYPOINT ["dotnet", "Gateway.dll"]