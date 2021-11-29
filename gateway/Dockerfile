FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /gateway

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY gateway/*.sln .
COPY gateway/src/Gateway/*.csproj ./src/Gateway/
RUN dotnet restore

# copy full solution over
COPY gateway .
RUN dotnet build

# publish the service
FROM build as publish
WORKDIR /gateway/src/Gateway
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /gateway
COPY --from=publish /gateway/src/Gateway/out ./
ENTRYPOINT ["dotnet", "Gateway.dll"]