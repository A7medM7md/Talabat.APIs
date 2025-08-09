using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        public Order()
        {
            
        } // This Empty Parameterless Constructor For EF Core To Able To Add Migration

        public Order(string buyerEmail, Address shippingAddress, decimal subtotal, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            Subtotal = subtotal;
            DeliveryMethod = deliveryMethod;
            Items = items;
            PaymentIntentId = paymentIntentId;
        }


        public string BuyerEmail { get; set; } // Email Of User That Makes The Order
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now; // The Ecommerce App Is Global In Many Countries, So We Need To Specify Offset Like UTC 
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }
        public decimal Subtotal { get; set; } // Is a Submission Of Price For Each Product as Item Multiply By It's Quantity -- تكلفة الاوردر منغير تكلفة التوصيل
       
        //[NotMapped] // Not Mapped To Column
        //public decimal Total => Subtotal + DeliveryMethod.Cost; // {ReadOnly Property} Subtotal + Delivery Cost [Derived Attribute -> Not Mapped As Column In DB and Computed In Runtime], If Computation Of It Is Complex It Should Be a Simple Attribute (Mapped To Column In DB) To Computed Once

        // We Can Write It As Method, To Avoid Writing Attributes
        public decimal GetTotal() 
            => Subtotal + DeliveryMethod.Cost; // This Is Driven Attribute, I Don't To Store It As a Column In DB, So I Made It As a Func, I Made It Also To Set Total Prop In OrderToReturnDto Class

        public string PaymentIntentId { get; set; }

        //public int DeliveryMethodId { get; set; } // FK

        public DeliveryMethod DeliveryMethod { get; set; } // Nav Prop [ONE]
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>(); // Nav Prop [MANY]
    }
}
