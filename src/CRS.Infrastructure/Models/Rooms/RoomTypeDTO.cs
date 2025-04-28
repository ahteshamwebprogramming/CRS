namespace CRS.Infrastructure.Models.Masters;

public class RoomTypeDTO
{
    public int Id { get; set; }

    public string? Rtype { get; set; }

    public int? Status { get; set; }

    public int? RoomRank { get; set; }
    public int NoOfRooms { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    private int _paxCount = 1;
    public int PaxCount
    {
        get => _paxCount;
        set => _paxCount = value > 3 ? 3 : value; 
    }
}
