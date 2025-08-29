namespace CRS.Infrastructure.Models.Masters
{
    public class TaskDTO
    {
        public int Id { get; set; }

        public string? TaskName { get; set; }

       // public DateTime? Duration { get; set; }

        public decimal? Rate { get; set; }
        public string? Remarks { get; set; }
    }
}
