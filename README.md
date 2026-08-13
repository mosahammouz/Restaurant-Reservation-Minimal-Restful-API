# RestaurantReservation.API

RestaurantReservation.API is a RESTful Web API built with **ASP.NET Core Minimal APIs** for managing restaurant reservations and related data.

The solution contains two projects:

- **RestaurantReservation.API** – Contains the Web API, authentication, DTOs, endpoints, validation, error handling, and Swagger/OpenAPI configuration.
- **RestaurantReservation.DB** – An existing project used for the database and data access.

## Features

- Restaurant reservation management
- Customer reservations
- Orders and menu items related to reservations
- Employee and manager information
- JWT-based authentication
- RSA asymmetric cryptography with RS256
- DTOs for API requests and responses
- Entity Framework Core
- SQL Server
- Request validation
- Error handling
- Swagger / OpenAPI
- API testing with Postman

## Authentication

The API uses **JWT (JSON Web Token)** authentication.

For signing JWTs, the project uses **RSA asymmetric cryptography with the RS256 algorithm**.

RSA uses a **private key** to sign the JWT and a **public key** to validate it.

The authentication flow is:

1. The user sends their login information.
2. The API verifies the user's credentials.
3. A JWT is generated and signed using the RSA private key.
4. The client receives the JWT.
5. The client sends the token with protected API requests.
6. The API validates the JWT using the RSA public key.
7. If the token is valid, the request is authorized.

## DTOs

The project uses **Data Transfer Objects (DTOs)** to control the data exchanged between the client and the API.

DTOs help prevent exposing database entities directly and allow the API to define exactly what data should be accepted or returned.

## Endpoints

The API provides the following endpoints:

### Authentication

- `POST /api/auth/login` – Authenticate a user and generate a JWT token.

### Reservations

- `GET /api/reservations` – Get all reservations.
- `GET /api/reservations/{reservationId}` – Get a reservation by ID.
- `POST /api/reservations` – Create a new reservation.
- `PUT /api/reservations/{reservationId}` – Update an existing reservation.
- `DELETE /api/reservations/{reservationId}` – Delete a reservation.
- `GET /api/reservations/customer/{customerId}` – Get reservations for a specific customer.
- `PATCH /api/reservations/{reservationId}` - Update a partial reservation

### Orders and Menu Items

- `GET /api/reservations/{reservationId}/orders` – Get the orders and menu items associated with a reservation.
- `GET /api/reservations/{reservationId}/menu-items` – Get the menu items ordered for a reservation.

### Employees

- `GET /api/employees/managers` – Get the managers.
- `GET /api/employees/{employeeId}/average-order-amount` – Get the average order amount for a specific employee.

Most endpoints require authentication using a valid JWT token. The login endpoint is used to obtain the token.

## Validation and Error Handling

The API includes **request validation and error handling** to make sure invalid requests are handled properly.

Validation is used to check incoming data before processing requests, while error handling provides appropriate responses when something goes wrong.

This helps make the API more reliable and easier to use.

## Swagger / OpenAPI

**Swagger / OpenAPI** is used to document and explore the API.

It provides information about the available endpoints, request parameters, request bodies, and responses.

Swagger can also be used to test the API directly from the documentation interface.

## Testing with Postman

**Postman** was used to test the API endpoints.

The authentication endpoint is tested first to obtain a JWT.

The JWT can then be included in the `Authorization` header when testing protected endpoints:

```text
Authorization: Bearer <JWT>