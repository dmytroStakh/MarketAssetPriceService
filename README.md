# MarketAssetPriceService

## Overview
MarketAssetPriceService is a web application for retrieving and managing market instruments. The service fetches data from an external API and stores it in a PostgreSQL database.

## Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Setup Instructions

### 1. Clone the Repository
git clone https://github.com/your-username/MarketAssetPriceService.git
cd MarketAssetPriceService

### 2.Run the Application
docker-compose up --build

### 3.Accessing the Application
Once the application is running, you can access the Swagger UI at:
http://localhost:5000/swagger/index.html

## Troubleshooting
### Common Issues

### 1. Environment Variables Not Set:
Ensure that the environment variables AUTH_TOKEN_URL, AUTH_USERNAME, and AUTH_PASSWORD are set correctly in your environment.

### 2. Database Connection Issues:
Ensure that the PostgreSQL container is running and accessible. Verify the connection string in the appsettings.json file.

### 3. Docker Issues:
Ensure Docker and Docker Compose are installed and running. If there are issues with container ports, try stopping all containers and restarting Docker.
```sh
