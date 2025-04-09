namespace CRS.Infrastructure.Models.Masters;

public class ServicesDTO
{
    public int Id { get; set; }

    public string? Service { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? MinimumNight { get; set; }
    public int? MaximumNight { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}
