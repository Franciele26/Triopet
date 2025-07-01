using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext.Entities
{
    public class Entry : ExtraEntity
    {
        public DateTime EntryDate { get; set; }
        public ICollection<ProductEntry> ProductEntries { get; set; } = new List<ProductEntry>();
    }
}
