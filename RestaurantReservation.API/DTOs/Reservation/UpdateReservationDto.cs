namespace RestaurantReservation.API.DTOs.Reservation;

public class UpdateReservationDto
{
   
        public DateTime ReservationDate { get; set; }
        public int PartySize { get; set; }
        public int TableId { get; set; }
    
}