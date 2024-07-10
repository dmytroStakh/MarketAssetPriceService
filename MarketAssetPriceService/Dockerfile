# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MarketAssetPriceService.csproj", "./"]
RUN dotnet restore "./MarketAssetPriceService.csproj"

# Install dotnet-ef tool version 6.0
RUN dotnet tool install --global dotnet-ef --version 6.0.0

# Ensure the dotnet tools path is available
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . .
WORKDIR "/src"
RUN dotnet build "MarketAssetPriceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarketAssetPriceService.csproj" -c Release -o /app/publish

# Use the SDK image to run EF Core migrations
FROM build AS efcore
WORKDIR /src

# Ensure the dotnet tools path is available in the efcore stage as well
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "ef"]

# Use the base image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarketAssetPriceService.dll"]
