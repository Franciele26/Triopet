using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models.DashBoardDtos
{
    public class ProductStockQuantityPerCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryDto Category { get; set; }
        public AnimalTypeDto AnimalType { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalInStock { get; set; }

    }
}
