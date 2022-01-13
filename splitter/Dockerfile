FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /splitter

# copy solution structure files and dependencies
# this is cached so rebuilding the image is faster on code changes
COPY splitter/*.sln .
COPY splitter/src/Splitter/*.csproj ./src/Splitter/
RUN dotnet restore

# copy full solution over
COPY splitter .
RUN dotnet build

# publish the service
FROM build as publish
WORKDIR /splitter/src/Splitter
RUN dotnet publish -c Release -o out

# run the service
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /splitter
COPY --from=publish /splitter/src/Splitter/out ./
ENTRYPOINT ["dotnet", "Splitter.dll"]