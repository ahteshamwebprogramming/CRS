using CRS.Infrastructure.Models.Masters;

namespace CRS.Infrastructure.ViewModels.Rooms
{
    public class RoomsViewModel
    {
        public List<RoomTypeWithChild>? RoomTypesWithAttr { get; set; }
        public List<ServicesDTO>? Services { get; set; }
        public List<RoomCalenderPrice>? RoomCalenderPrices { get; set; }
        public List<TaskDTO>? TaskPackage { get; set; }
    }
}
