FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY *.sln .
COPY src/ParkingService/*.csproj ./src/ParkingService/
COPY test/ParkingServiceTest/*.csproj ./test/ParkingServiceTest/
RUN dotnet restore

# copy full solution over
COPY . .
RUN dotnet build

# create unit test layer
FROM build AS testrunner
WORKDIR /app/test/ParkingServiceTest
CMD ["dotnet", "test", "--logger:trx"]

# run the unit tests
FROM build AS test
WORKDIR /app/test/ParkingServiceTest
RUN dotnet test --logger:trx

# publish the service
FROM test as publish
WORKDIR /app/src/ParkingService
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /app/src/ParkingService/out ./
ENTRYPOINT ["dotnet", "ParkingService.dll"]