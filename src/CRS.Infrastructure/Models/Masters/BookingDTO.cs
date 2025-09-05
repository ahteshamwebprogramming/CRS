using System.ComponentModel.DataAnnotations;

namespace CRS.Infrastructure.Models.Masters
{
    public class BookingDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

       
        public string Address { get; set; }

        public int? CountryId { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string ZipCode { get; set; }

        // List of Guests
        public List<GuestDetailsDTO> Guests { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
