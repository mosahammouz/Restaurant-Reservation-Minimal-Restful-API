using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Data;

namespace RestaurantReservation.API.Endpoints;

public static class ReservationEndpoints
{
    public static async Task<IResult> GetReservations(RestaurantReservationDbContext db) // IResult is the HTTP response
    {
        var reservations = await db.Reservations.ToListAsync();
        return Results.Ok(reservations);
    }

    public static async Task<IResult> GetReservationsById(RestaurantReservationDbContext db, int reservationId)
    {
        var reservation = await db.Reservations.FindAsync(reservationId); // if not exist return null
        if (reservation == null)
        {
            return Results.NotFound($"Reservation with {reservationId} id is not found");
        }

        return Results.Ok(reservation);
    }

    public static async Task<IResult> CreateReservation(RestaurantReservationDbContext db, Reservation reservation)
    {
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();
        return Results.Created($"/api/reservations/{reservation.ReservationId}",reservation); //201 and give the client the url and the reservation obj as a json 
    }
}