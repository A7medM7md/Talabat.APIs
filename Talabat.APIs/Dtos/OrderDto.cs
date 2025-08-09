using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class OrderDto
    {
        [Required]
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; } // Already Required
        public AddressDto ShippingAddress { get; set; } // Make AddressDto and Make It's Attributes Required
    }
}
