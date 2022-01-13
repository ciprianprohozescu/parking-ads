FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /aggregator

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY aggregator/*.sln .
COPY aggregator/src/Aggregator/*.csproj ./src/Aggregator/
RUN dotnet restore

# copy full solution over
COPY aggregator .
RUN dotnet build

# publish the service
FROM build as publish
WORKDIR /aggregator/src/Aggregator
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /aggregator
COPY --from=publish /aggregator/src/Aggregator/out ./
ENTRYPOINT ["dotnet", "Aggregator.dll"]