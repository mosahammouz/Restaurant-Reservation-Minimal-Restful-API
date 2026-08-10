using Microsoft.EntityFrameworkCore;
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

app.Run();