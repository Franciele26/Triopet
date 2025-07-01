using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext.Entities
{
    public class AnimalType : ExtraEntity
    {
        public string Type { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
