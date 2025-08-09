using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Config
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Store OrderStatus In DB As String, And When Returned, Return It's Type To OrderStatus Type
            builder.Property(O => O.Status).HasConversion(
                
                OStatus => OStatus.ToString(), /* Take The Value In EnumMember("") Attribute */
                OStatus => (OrderStatus) Enum.Parse(typeof(OrderStatus), OStatus) // Not Necessary To Write OStatus As The Same Name Above, Write Any Name You Want
            );
            
            // Map Address With Owner (Order) As Columns In Order Table [Same Table]
            builder.OwnsOne(O => O.ShippingAddress, A => A.WithOwner()); // 1:1 Total

            builder.Property(O => O.Subtotal).HasColumnType("decimal(18,2)");


            builder.HasMany(O => O.Items)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
