namespace CRS.Infrastructure.ViewModels.Rooms
{
    public class RoomTypeWithChild
    {
        public int Id { get; set; }

        public string? Rtype { get; set; }

        public int? Status { get; set; }

        public int? RoomRank { get; set; }
        public int? NoOfRooms { get; set; }
        public string? Remarks { get; set; }
        public decimal? OfferedPrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public decimal? OfferPercentage { get; set; }
    }
}
