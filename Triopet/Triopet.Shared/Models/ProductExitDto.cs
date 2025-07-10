using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models
{
    public class ProductExitDto
    {
        public int ProductId { get; set; }
        public int ExitId { get; set; }
        public int Quantity { get; set; }
        public ProductDto? Product { get; set; } 
    }
}
