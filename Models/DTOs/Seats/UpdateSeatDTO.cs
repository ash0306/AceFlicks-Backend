namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class UpdateSeatDTO
    {
        public int Id { get; set; }
        public string SeatStatus { get; set; }
        public bool IsAvailable { get; set; }
        public int? BookingId { get; set; }
    }
}
