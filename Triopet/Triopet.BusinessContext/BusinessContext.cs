using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.BusinessContext
{
    public class BusinessContext : DbContext, IBusinessContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=SQL1004.site4now.net;Initial Catalog=db_abaa30_dotnet003;User Id=db_abaa30_dotnet003_admin;Password=YourstrongP@ssword1");
        }
    }
}
