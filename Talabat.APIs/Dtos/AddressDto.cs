using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class AddressDto
    {
        //public int Id { get; set; } // I Don't Need This Prop In Dto // Has Identity Constraints, User Not Set/Update It

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

    }
}
