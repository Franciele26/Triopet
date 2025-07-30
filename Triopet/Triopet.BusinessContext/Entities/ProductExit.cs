using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext.Entities
{
    public class ProductExit : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ExitId { get; set; }
        public Exit Exit { get; set; }
        public int Quantity { get; set; }
    }
}
