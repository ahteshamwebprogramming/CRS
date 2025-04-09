namespace CRS.Infrastructure.ViewModels.Summary
{
    public class SummaryInputDTO
    {
        public string? RoomTypes { get; set; }
        public int? RoomType { get; set; }
        public int? NoOfRooms { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
    }
}
