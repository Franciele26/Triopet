using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triopet.BusinessContext.Entities;

namespace Triopet.BusinessContext
{
    public class BusinessContext : DbContext, IBusinessContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=SQL1004.site4now.net;Initial Catalog=db_abaa30_dotnet003;User Id=db_abaa30_dotnet003_admin;Password=YourstrongP@ssword1");
        }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Exit> Exits { get; set; }
        public DbSet<Motif> Motifs { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductEntry> ProductEntries { get; set; }
        public DbSet<ProductExit> ProductExits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntry>()
                .HasKey(pe => new { pe.EntryId, pe.ProductId });
            modelBuilder.Entity<ProductExit>()
                .HasKey(pe=> new { pe.ExitId, pe.ProductId });
            base.OnModelCreating(modelBuilder);
           
        }
    }
}
