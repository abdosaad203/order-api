# Order API

Order management microservice for the e-commerce platform.

## Features

* Create orders
* View orders
* Cancel orders
* JWT authentication
* Prometheus metrics
* Health checks
* MySQL integration

## Tech Stack

* ASP.NET Core 10
* Entity Framework Core
* MySQL
* Docker

## Run with Docker

### Build Image

```bash
docker build -t order-api:v1 .
```

### Run Container

```bash
docker run -d \
--name order-api-dev \
-p 8082:8082 \
-e ASPNETCORE_URLS=http://+:8082 \
-e ConnectionStrings__MySQL="Server=host.docker.internal;Port=3306;Database=ecommerce;User=root;Password=rootpass123;" \
-e CatalogApi__BaseUrl="http://host.docker.internal:8081" \
-e Jwt__Key="ThisIsA32CharOrLongerSuperSecretKey!2025" \
-e Jwt__Issuer="ecommerce-identity" \
-e Jwt__Audience="ecommerce" \
order-api:v1
```

## Endpoints

* `/api/orders`
* `/health`
* `/metrics`

## Port

* 8082
