# Mandiri Foods
Sample project for mandiri technical task.

## Requirements
- Microservice
- Entity Relationship (1-1, 1-N, N-N)
- Rest API
- Transactional function

## Features
- CRUD Food catalogue
- CRUD Cuisine as food categories (N - N)
- CRUD User and Password on separate table (1 - 1)
- Transaction (1 - N)

## Tech
- Standard .NET 7 web api framework
- JWT Bearer Authentication with custom UMS
- Code first .NET EF db migration
- Symmetric encryption for stored password (AES)
- Paging with metadata
- Natural language AI with third party API (spoonacular)
- Changelog / AuditTrail for CRUD Entities
- DTO with AutoMapper
- Swagger UI

## Architecture
![alt text](https://camo.githubusercontent.com/9d0150e6c15f0895d4203596623197455a9ce342d5e8e17ef0645e6e456ee2eb/68747470733a2f2f692e696d6775722e636f6d2f594d55354430482e706e67)

## Installation
Depedencies
- .NET 7
- MS SQL
- Entity Framework
```sh
dotnet ef database update
dotnet build
dotnet run
```


