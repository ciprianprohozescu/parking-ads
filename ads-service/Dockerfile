FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /ads-service

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY ads-service/*.sln .
COPY ads-service/src/AdsService/*.csproj ./src/AdsService/
RUN dotnet restore

# copy full solution over
COPY ads-service .
RUN dotnet build

# publish the service
FROM build as publish
WORKDIR /ads-service/src/AdsService
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /ads-service
COPY --from=publish /ads-service/src/AdsService/out ./
ENTRYPOINT ["dotnet", "AdsService.dll"]