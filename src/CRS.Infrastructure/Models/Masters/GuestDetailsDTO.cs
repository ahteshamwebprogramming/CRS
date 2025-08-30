using System.ComponentModel.DataAnnotations;

namespace CRS.Infrastructure.Models.Masters
{
    public class GuestDetailsDTO
    {
        [Required]
        public string? FirstName { get; set; }

        
        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

       
        public string? Gender { get; set; }

        public int? Age { get; set; }
    }
}
