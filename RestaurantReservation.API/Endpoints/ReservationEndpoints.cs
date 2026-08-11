using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.DTOs.Reservation;
using RestaurantReservation.Db.Data;
using RestaurantReservation.Db.Models;

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
    
    public static async Task<IResult> CreateReservation(RestaurantReservationDbContext db, CreateReservationDto dto)
    {
        var reservation = new Reservation
        {
            CustomerId = dto.CustomerId,
            RestaurantId = dto.RestaurantId,
            TableId = dto.TableId,
            ReservationDate = dto.ReservationDate,
            PartySize = dto.PartySize
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        var reservationDto = new ReservationDto
        {
            ReservationId = reservation.ReservationId,
            CustomerId = reservation.CustomerId,
            RestaurantId = reservation.RestaurantId,
            TableId = reservation.TableId,
            ReservationDate = reservation.ReservationDate,
            PartySize = reservation.PartySize
        };

        return Results.Created(
            $"/api/reservations/{reservation.ReservationId}",
            reservationDto);
    }
    
    public static async Task<IResult> UpdateReservation(RestaurantReservationDbContext db, UpdateReservationDto dto, int reservationId)
    {
        var existingReservation = await db.Reservations.FindAsync(reservationId);

        if (existingReservation == null)
        {
            return Results.NotFound(
                $"Reservation with {reservationId} id is not found");
        }

        existingReservation.ReservationDate = dto.ReservationDate;
        existingReservation.PartySize = dto.PartySize;
        existingReservation.TableId = dto.TableId;

        await db.SaveChangesAsync();

        var reservationDto = new ReservationDto
        {
            ReservationId = existingReservation.ReservationId,
            CustomerId = existingReservation.CustomerId,
            RestaurantId = existingReservation.RestaurantId,
            TableId = existingReservation.TableId,
            ReservationDate = existingReservation.ReservationDate,
            PartySize = existingReservation.PartySize
        };

        return Results.Ok(new
        {
            message = "Reservation has been updated successfully",
            reservation = reservationDto
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

    public static async Task<IResult> GetOrderedMenuItemsByReservationId(RestaurantReservationDbContext db , int reservationId)
    {
        var existingReservation = await db.Reservations.AnyAsync(r => r.ReservationId == reservationId);
        if(!existingReservation){return Results.NotFound($"Reservation with {reservationId} id is NOT found");}

        var menuItems = await db.Orders.Where(o => o.ReservationId == reservationId)
            .SelectMany(o => o.OrderItems)
            .Select(oi => new
                {
                    oi.MenuItemId,
                    oi.Quantity,
                    MenuItem = new
                    {
                        oi.MenuItem.MenuItemId,
                        oi.MenuItem.Name,
                        oi.MenuItem.Price
                    }
                }
            ).ToListAsync();
        return Results.Ok(menuItems);
    }

    public static async Task<IResult> AvgOrderAmountByEmployeeId(RestaurantReservationDbContext db , int employeeId)
    {
        var existingEmployee = await db.Employees.AnyAsync(e => e.EmployeeId == employeeId);
        if(!existingEmployee){return Results.NotFound($"Employee with ID {employeeId} wasn't found");}

        var avg = await db.Orders.Where(o => o.EmployeeId == employeeId).AverageAsync(o => o.TotalAmount);
        return Results.Ok(new { employeeId, AvgOrderAmount = avg });
    }

    public static async Task<IResult> DeleteReservation(RestaurantReservationDbContext db, int reservationId)
    {
        var reservation = await db.Reservations.FindAsync(reservationId);
        if (reservation == null) { return Results.NotFound($"Reservation with id {reservationId} was not found"); }

        db.Reservations.Remove(reservation);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Reservation has been deleted successfully" });
    }
}