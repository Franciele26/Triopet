using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triopet.BusinessContext.Entities;

namespace Triopet.BusinessContext
{
    public interface IBusinessContext
    {
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Exit> Exits { get; set; }
        public DbSet<Motif> Motifs { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductEntry>ProductEntries { get; set; }
        public DbSet<ProductExit> ProductExits { get; set; }

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    }
}
