namespace CRS.Infrastructure.Models.Rooms
{
    public class AvailabilityCalender
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? Rtype { get; set; }
        public int? PackageId { get; set; }
    }
}
