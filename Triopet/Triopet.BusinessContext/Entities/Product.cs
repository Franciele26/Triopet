using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext.Entities
{
    public class Product : ExtraEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get;set; }
        public int AnimalTypeId { get; set; }
        public Category Category { get; set; }
        public AnimalType AnimalType { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<ProductEntry> ProductEntries { get; set; } = new List<ProductEntry>();
        public ICollection<ProductExit> ProductExits { get; set; } = new List<ProductExit>();

    }
}
