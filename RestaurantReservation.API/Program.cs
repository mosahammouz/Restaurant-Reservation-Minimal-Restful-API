using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Endpoints;
using RestaurantReservation.Db.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RestaurantReservationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
// testing the connection 
app.MapGet("/test-db", async (RestaurantReservationDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();

    return canConnect
        ? Results.Ok("Database connection successful!")
        : Results.Problem("Could not connect to database.");
});
// testing Endpoints 
app.MapGet("/api/reservations", ReservationEndpoints.GetReservations); // without () means when someone sends a request call this func
                                                                       // with () means this func call it now even if nobody sends a request
app.MapGet("/api/reservations/{reservationId}",ReservationEndpoints.GetReservationsById);// cuz its collection !
app.MapPost("/api/reservations", ReservationEndpoints.CreateReservation);
app.MapPut("/api/reservations/{reservationId}", ReservationEndpoints.UpdateReservation);
app.MapGet("/api/employees/managers", ReservationEndpoints.GetManagers);
app.MapGet("/api/reservations/customer/{customerId}", ReservationEndpoints.GetReservationsByCustomerId);
app.Run();