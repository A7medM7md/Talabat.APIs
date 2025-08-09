using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Dtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } // مش محتاج ادي ديفولت فاليو هنا لانه بياخدها لما اكريت الاوردر ولما اعمل مابينج بتبقا معايا الفاليو دي
        public string Status { get; set; }
        public Address ShippingAddress { get; set; }
        public decimal Subtotal { get; set; }
        public string DeliveryMethod { get; set; } // DeliveryMethodName
        public string DeliveryMethodCost { get; set; }
        public decimal Total { get; set; } // Take It's Value From Func In Original Entity When Mapping [Implicitly]
        public string PaymentIntentId { get; set; } = string.Empty;
        public ICollection<OrderItemDto> Items { get; set; }

    }
}
