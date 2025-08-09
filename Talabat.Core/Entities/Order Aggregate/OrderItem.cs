using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    /* The Most Important Component In Order Module */
    public class OrderItem : BaseEntity /* OrderItem Refers To Product Exists In Order As an Item */
    {
        public OrderItem()
        {
            
        }
        public OrderItem(ProductItemOrdered product, decimal price, int quantity)
        {
            Product = product;
            Price = price;
            Quantity = quantity;
        }

        // We Need Info About Product That Delivered As Item In Order
        //public int ProductId { get; set; }
        //public string ProductName { get; set; }
        //public string PictureUrl { get; set; }

        // Above Three Info Is Fixed, Not Changed لو اتنين طلبوا نفس الايتم فال 3 حاجات دول ثابتين
        // So We Can PutThem In Obj
        // Quantity Chosen By The User and Price Changing According To Quantity or Discount -> This Price Is Not Product Price, But Is The Price Of Product As Item In Order - سعر البرودكت نفسه ثابت انما دا حاجة تانية

        public ProductItemOrdered Product { get; set; } // Info About Product That Delivered As Item In Order

        public decimal Price { get; set; }
        public int Quantity { get; set; } // هنطلب قد اي من البرودكت دا كأيتم جوه الاوردر

        // OrderItem Belongs To One Order
        //public int OrderId { get; set; } // FK {I Don't Need A Representations For It Here, EF Will Create It In DB}
        //public Order Order { get; set; } // Nav Prop [ONE] {Don't Need To Navigate On Orders From OrderItem}
    }
}
