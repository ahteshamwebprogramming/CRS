using CRS.Infrastructure.Models.Masters;

namespace CRS.Infrastructure.ViewModels.Summary
{
    public class SummaryViewModel
    {
        public SummaryInputDTO? SummaryInputDTO { get; set; }
        public RoomTypeDTO? RoomTypeDTO { get; set; } = new();
    }

    public class RoomSelectionDTO
    {
        public RoomTypeDTO RoomType { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
