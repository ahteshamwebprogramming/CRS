using System.ComponentModel.DataAnnotations.Schema;

namespace CRS.Core.Entities;

[Dapper.Contrib.Extensions.Table("GenOperations")]
public class GenOperations
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? opt { get; set; }
}
