FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /parking-service

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY parking-service/*.sln .
COPY parking-service/src/ParkingService/*.csproj ./src/ParkingService/
COPY parking-service/test/ParkingServiceTest/*.csproj ./test/ParkingServiceTest/
RUN dotnet restore

# copy full solution over
COPY parking-service .
RUN dotnet build

# create unit test layer
FROM build AS testrunner
WORKDIR /parking-service/test/ParkingServiceTest
CMD ["dotnet", "test", "--logger:trx"]

# run the unit tests
FROM build AS test
WORKDIR /parking-service/test/ParkingServiceTest
RUN dotnet test --logger:trx

# publish the service
FROM test as publish
WORKDIR /parking-service/src/ParkingService
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /parking-service
COPY --from=publish /parking-service/src/ParkingService/out ./
ENTRYPOINT ["dotnet", "ParkingService.dll"]