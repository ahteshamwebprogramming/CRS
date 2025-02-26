using System.ComponentModel.DataAnnotations.Schema;

namespace CRS.Core.Entities;
[Dapper.Contrib.Extensions.Table("RoomType")]

public class RoomType
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Rtype { get; set; }

    public int? Status { get; set; }

    public int? RoomRank { get; set; }
}
