using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderSpecifications : BaseSpecification<Order>
    {
        // This Ctor Is Used For Get List Of Orders
        public OrderSpecifications(string email) : base(O => O.BuyerEmail == email) // Chain On Ctor That Takes Criteria
        {
            // Includes [Load Nav Props] // => Eager Loading (Done For Nav Prop From Type One Not Many) , While Items Is Nav Prop From Type Many, I'll Do Lazy Loading For It, But Here I Need To Load Items With Order Always, So I Load Items [Eager Loading]
            Includes.Add(O => O.DeliveryMethod); // Eager Loading
            Includes.Add(O => O.Items); // Eager Loading [حاله شاذه] -> We Always Retrieve Items In The Same Request [I Always Need It]

            AddOrderByDesc(O => O.OrderDate);

            // If You Need To Apply Pagination
            //ApplyPagenation()
        }

        // This Ctor Is Used For Get Specific Order
        public OrderSpecifications(string email, int id) : base(O => O.BuyerEmail == email && O.Id == id)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }

    }
}
