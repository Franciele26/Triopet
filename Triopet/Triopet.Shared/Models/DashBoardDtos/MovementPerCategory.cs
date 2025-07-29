using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models.DashBoardDtos
{
    public class MovementPerCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int NumberMovements { get; set; }
    }
}
