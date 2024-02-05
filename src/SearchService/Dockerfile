FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
EXPOSE 80


COPY WheelDealBids.sln WheelDealBids.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/BuildingBlocks/BuildingBlocks.csproj src/BuildingBlocks/BuildingBlocks.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY tests/UnitTests/UnitTests.csproj tests/UnitTests/UnitTests.csproj
COPY tests/IntegrationTests/IntegrationTests.csproj tests/IntegrationTests/IntegrationTests.csproj

RUN dotnet restore WheelDealBids.sln

COPY src/SearchService src/SearchService
COPY src/BuildingBlocks src/BuildingBlocks
WORKDIR /app/src/SearchService
RUN dotnet publish -c Release -o /app/src/out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "SearchService.dll" ]