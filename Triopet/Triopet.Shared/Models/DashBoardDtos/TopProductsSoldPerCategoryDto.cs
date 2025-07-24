using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models.DashBoardDtos
{
    public class TopProductsSoldPerCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SoldQuantity { get; set; }
    }
}
