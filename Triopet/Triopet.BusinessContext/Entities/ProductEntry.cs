using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext.Entities
{
    public class ProductEntry: BaseEntity
    {
        public int EntryId { get; set; }
        public Entry Entry { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal PriceUnit { get; set; }
    }
}
