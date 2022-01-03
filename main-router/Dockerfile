FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /main-router

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY main-router/*.sln .
COPY main-router/src/MainRouter/*.csproj ./src/MainRouter/
RUN dotnet restore

# copy full solution over
COPY main-router .
RUN dotnet build

# publish the service
FROM build as publish
WORKDIR /main-router/src/MainRouter
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /main-router
COPY --from=publish /main-router/src/MainRouter/out ./
ENTRYPOINT ["dotnet", "MainRouter.dll"]