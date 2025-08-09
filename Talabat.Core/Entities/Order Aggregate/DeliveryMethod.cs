using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    // It Will Be a Table In DB
    public class DeliveryMethod : BaseEntity
    {
        public DeliveryMethod()
        {
            
        }

        public DeliveryMethod(string shortName, string description, decimal cost, string deliveryTime)
        {
            ShortName = shortName;
            Description = description;
            Cost = cost;
            DeliveryTime = deliveryTime;
        }

        public string ShortName { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string DeliveryTime { get; set; } // In Day Periods [Delivery In 2 Days To 5 Days]

        //public ICollection<Order> Orders { get; set; } // Nav Prop [MANY] {Don't Need To Know Orders Associated With Delivery Method}
    }
}
