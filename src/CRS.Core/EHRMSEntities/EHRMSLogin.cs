using System.ComponentModel.DataAnnotations.Schema;

namespace CRS.Core.EHRMSEntities;

[Dapper.Contrib.Extensions.Table("EHRMSLogin")]
public class EHRMSLogin
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LoginId { get; set; }

    public string WorkerCode { get; set; } = null!;

    public string? UserPassword { get; set; }

    public bool? IsActive { get; set; }

    public decimal? WorkerId { get; set; }

    public int? LogintCount { get; set; }
}
