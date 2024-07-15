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
```sh
