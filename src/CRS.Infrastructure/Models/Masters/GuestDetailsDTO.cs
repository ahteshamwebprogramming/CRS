using System.ComponentModel.DataAnnotations;

namespace CRS.Infrastructure.Models.Masters
{
    public class GuestDetailsDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string Gender { get; set; }

        public int? Age { get; set; }
    }
}
