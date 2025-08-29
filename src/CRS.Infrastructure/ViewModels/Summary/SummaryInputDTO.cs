using CRS.Infrastructure.Models.Masters;

namespace CRS.Infrastructure.ViewModels.Summary
{
    public class SummaryInputDTO
    {
        public string? RoomTypes { get; set; }
        public int? RoomType { get; set; }
        public int? NoOfRooms { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NoOfNights { get; set; }
        //public Dictionary<string, int> RoomSelections { get; set; } = new();
        public Dictionary<string, RoomSelectionDTO> RoomSelections { get; set; } = new();
        public List<RoomTypeDTO> SelectedRooms { get; set; } = new();
        public List<PaxInfo> PaxPerRoom { get; set; } = new();
        public List<ServiceDTO> SelectedServices { get; set; } = new();

        public BookingDTO? BookingDetails { get; set; }
    }
    public class PaxInfo
    {
        public int RoomNumber { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
    public class ServiceDTO
    {
        public string Service { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
