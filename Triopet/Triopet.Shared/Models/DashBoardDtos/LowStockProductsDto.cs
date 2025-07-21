using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models.DashBoardDtos
{
    public class LowStockProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public int AnimalTypeId { get; set; }
        public AnimalTypeDto AnimalType { get; set; }
        public int QUantity { get; set; }

    }
}
