using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantReservation.API.Authentication;
using RestaurantReservation.API.Endpoints;
using RestaurantReservation.Db.Data;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        if (context.Description.ActionDescriptor.EndpointMetadata
            .OfType<IAuthorizeData>()
            .Any())
        {
            operation.Security ??= [];

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer",
                        context.Document)] = []
                });
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<RestaurantReservationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var rsa = RSA.Create(2048);

builder.Services.AddSingleton(rsa); // it will add rsa to the DI Container
builder.Services.AddSingleton<JwtTokenGenerator>();// whenever someone asks for JwtTokenGenerator type create or use the obj in the DI container (creating one from the constructor and it will reuser the RSA instance for the para. from DI container)

builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new RsaSecurityKey(
                rsa.ExportParameters(false)),

            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Restaurant Reservation API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

// testing the connection
app.MapGet("/test-db", async (RestaurantReservationDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();

    return canConnect
        ? Results.Ok("Database connection successful!")
        : Results.Problem("Could not connect to database.");
});
//Groups
var authGroup = app.MapGroup("/api/auth");
var reservationsGroup = app.MapGroup("/api/reservations").RequireAuthorization();
var employeesGroup = app.MapGroup("/api/employees").RequireAuthorization();
// testing Endpoints
authGroup.MapPost("/login", AuthEndpoints.Login);
reservationsGroup.MapPut("/{reservationId}", ReservationEndpoints.UpdateReservation);
reservationsGroup.MapGet("/", ReservationEndpoints.GetReservations);
reservationsGroup.MapGet("/customer/{customerId}", ReservationEndpoints.GetReservationsByCustomerId);
reservationsGroup.MapGet("/{reservationId}", ReservationEndpoints.GetReservationsById);
reservationsGroup.MapPost("/", ReservationEndpoints.CreateReservation);
employeesGroup.MapGet("/managers", ReservationEndpoints.GetManagers);
reservationsGroup.MapGet("/{reservationId}/orders", ReservationEndpoints.GetOrdersAndMenuItemsByReservationId);
employeesGroup.MapGet("/{employeeId}/average-order-amount", ReservationEndpoints.AvgOrderAmountByEmployeeId);
reservationsGroup.MapGet("/{reservationId}/menu-items", ReservationEndpoints.GetOrderedMenuItemsByReservationId);
reservationsGroup.MapDelete("/{reservationId}", ReservationEndpoints.DeleteReservation);
reservationsGroup.MapPatch("/{reservationId}", ReservationEndpoints.PatchReservation);
app.Run();