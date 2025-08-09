using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int ProductBrandId { get; set; } // FK : Not Allow NULL -> [By Default -> On Delete Cascade]
        public int ProductTypeId { get; set; } // FK : Not Allow NULL -> [By Default -> On Delete Cascade]

        // Navigational Properties
        public ProductBrand ProductBrand { get; set; } // [ONE] // One Product Belongs To One Brand & One Brand Has Many Products
        public ProductType ProductType { get; set; } // [ONE] // One Product Belongs To One Type
    }
}
