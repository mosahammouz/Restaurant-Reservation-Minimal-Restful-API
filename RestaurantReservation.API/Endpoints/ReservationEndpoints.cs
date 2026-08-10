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

    public static async Task<IResult> UpdateReservation(RestaurantReservationDbContext db,Reservation reservation ,int reservationId)
    {
        var existingReservation  = await db.Reservations.FindAsync(reservationId); // if not exist return null
        if (existingReservation  == null)
        {
            return Results.NotFound($"Reservation with {reservation.ReservationId} id is not found");
        }
        //existingReservation is already existed, so we are updating it
        existingReservation.CustomerId = reservation.CustomerId;
        existingReservation.RestaurantId = reservation.RestaurantId;
        existingReservation.TableId = reservation.TableId;
        existingReservation.ReservationDate = reservation.ReservationDate;
        existingReservation.PartySize = reservation.PartySize;
        await db.SaveChangesAsync();
        return Results.Ok(new
        {
            message = "Reservations has been updated successfully",
            reservation  =existingReservation
        });

    }

    public static async Task<IResult> GetManagers(RestaurantReservationDbContext db)
    {
        var managers = await db.Employees.Where(e => e.Position == "Manager").ToListAsync();
        return Results.Ok(managers);
    }

    public static async Task<IResult> GetReservationsByCustomerId(RestaurantReservationDbContext db, int customerId)
    {
        var customerExists = await db.Customers.AnyAsync(c => c.CustomerId == customerId);
        if (!customerExists) { return Results.NotFound($"Customer with ID {customerId} was not found."); }
        var reservations = await db.Reservations.Where(r => r.CustomerId == customerId).ToListAsync();
        return Results.Ok(reservations);
    }

    public static async Task<IResult> GetOrdersAndMenuItemsByReservationId(RestaurantReservationDbContext db, int reservationId)
    {
        var reservationExists = await db.Reservations.AnyAsync(r => r.ReservationId == reservationId);
        if (!reservationExists) { return Results.NotFound($"Reservation with ID {reservationId} was not found."); }

        var orders = await db.Orders
            .Where(o => o.ReservationId == reservationId)
            .Select(o => new
            {
                o.OrderId,
                o.ReservationId,
                o.TotalAmount,

                OrderItems = o.OrderItems.Select(oi => new // a collection
                {
                    oi.OrderItemId,
                    oi.MenuItemId,
                    oi.Quantity,
                    MenuItem = new // single one
                    {
                        oi.MenuItem.MenuItemId,
                        oi.MenuItem.Name,
                        oi.MenuItem.Price
                    }
                })
            })
            .ToListAsync();
        return Results.Ok(orders);
    }
}