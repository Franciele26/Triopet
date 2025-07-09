using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext.Entities
{
    public class Exit : ExtraEntity
    {
        public DateTime ExitDate { get; set; }
        public int MotifId { get; set; }
        public Motif Motif { get; set; }
        public ICollection<ProductExit> ProductExits { get; set; } = new List<ProductExit>();
        public int Quantity { get; set; }
    }
}
