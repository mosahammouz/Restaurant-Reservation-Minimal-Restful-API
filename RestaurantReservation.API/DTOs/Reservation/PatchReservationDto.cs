namespace RestaurantReservation.API.DTOs.Reservation;

public class PatchReservationDto // client can send either both or one of them or noting
{
    public int? TableId { get; set; } 
    public DateTime? ReservationDate { get; set; }
}