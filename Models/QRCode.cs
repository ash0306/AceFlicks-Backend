namespace MovieBookingBackend.Models
{
    public class QRCode
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public byte[] BookingQR {  get; set; }
        public Booking Booking {  get; set; }
    }
}
