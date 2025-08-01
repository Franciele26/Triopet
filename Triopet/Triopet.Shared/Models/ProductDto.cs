﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Decimal PricePerUnit { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public int AnimalTypeId { get; set; }
        public AnimalTypeDto AnimalType { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        public int Quantity { get; set; }
    }
}
