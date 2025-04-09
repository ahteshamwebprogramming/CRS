using System.ComponentModel.DataAnnotations.Schema;

namespace CRS.Core.Entities;
[Dapper.Contrib.Extensions.Table("Services")]
public class Services
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Service { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateDate { get; set; }
    public int? MinimumNight { get; set; }
    public int? MaximumNight { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}
