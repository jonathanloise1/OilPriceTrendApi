# OilPriceTrendApi

## Description

OilPriceTrendApi is an application that provides a JSON-RPC endpoint to retrieve the historical trend of oil prices. It uses publicly available historical data of Brent Europe to respond to user requests.

## Features

- **GetOilPriceTrend**: Retrieves the historical trend of oil prices for a specified period.
- **Caching**: Utilizes in-memory caching to improve performance by reducing external data calls.
- **Validation**: Uses FluentValidation to ensure the correctness of input data.
- **Logging**: Implements NLog for detailed logging and error tracking.

## Requirements

- .NET 8
- Docker (optional, for containerization)

## Getting Started

### Prerequisites

Make sure you have the following installed on your machine:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop) (if you want to run the application in a container)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/OilPriceTrendApi.git
   cd OilPriceTrendApi

2. **Restore the packages**
   ```bash
   dotnet restore

3. **Run the application**
   ```bash
   dotnet run

### Running with Docker

1. **Build the Docker image**
   ```bash
   docker build -t oilpricetrendapi .

2. **Run the Docker container**
   ```bash
   docker run -p 8080:80 oilpricetrendapi

### Configuration

The application uses an `appsettings.json` file for configuration. Key settings include:

- `OilPriceTrendsConfig:Url`: URL to fetch the historical oil price data.
- `OilPriceTrendsConfig:HttpClientName`: Name of the HTTP client used to fetch data.
- `OilPriceTrendsConfig:CacheDurationInMilliseconds`: Duration (in milliseconds) for how long the data should be cached.

### Usage

Once the application is running, you can use tools like `curl` or Postman to interact with the JSON-RPC endpoint. Below is an example request payload:

```json
{
  "id": 1,
  "jsonrpc": "2.0",
  "method": "GetOilPriceTrend",
  "params": {
    "startDateISO8601": "2020-01-01",
    "endDateISO8601": "2020-01-05"
  }
}
```

### Example Response

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "prices": [
      {
        "dateISO8601": "2020-01-01",
        "price": 12.3
      },
      {
        "dateISO8601": "2020-01-02",
        "price": 13.4
      },
      {
        "dateISO8601": "2020-01-03",
        "price": 14.5
      },
      {
        "dateISO8601": "2020-01-04",
        "price": 16.7
      },
      {
        "dateISO8601": "2020-01-05",
        "price": 18.9
      }
    ]
  }
}
```

### Testing

This project includes unit tests for the services and API. To run the tests, use the following command:

```bash
dotnet test
```

### Logging

The application uses NLog for logging. Logs are configured to be written to a file located at `Logs/oilprice-api-{Date}.txt`. You can adjust the logging configuration in the `nlog.config` file to fit your requirements.

### Contributing

Contributions are welcome! If you have suggestions for improvements or new features, feel free to open an issue or submit a pull request. Please ensure your code follows the existing code style and includes appropriate tests.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
