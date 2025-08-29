using CRS.Infrastructure.Models.Masters;

namespace CRS.Infrastructure.ViewModels.Summary
{
    public class SummaryViewModel
    {
        public SummaryInputDTO? SummaryInputDTO { get; set; }
        public RoomTypeDTO? RoomTypeDTO { get; set; } = new();

        public string? Source { get; set; }

        public List<RoomSelectionDTO1>? RoomSelectionList { get; set; }
    }
    public class SummaryViewModelNew
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public int? NoOfNights { get; set; }

        public List<RoomSelectionDTO1>? RoomSelectionList { get; set; }

        public List<ServiceDTO> SelectedServices { get; set; } = new();

    }

    public class RoomSelectionDTO
    {
        public RoomTypeDTO RoomType { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
    public class RoomSelectionDTO1
    {
        public int? RoomNumber { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }

        public List<ServiceDTO> SelectedServices { get; set; } = new();

        public RoomDetails? RoomDetails { get; set; }

    }
    public class RoomDetails
    {
        public int? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public decimal? Price { get; set; }
    }

}
