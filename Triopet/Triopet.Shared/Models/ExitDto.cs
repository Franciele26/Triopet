using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triopet.Shared.Models
{
    public class ExitDto
    {
        public int Id { get; set; }
        public DateTime DateOfExit { get; set; }
        public int ReasonId { get; set; }
        public ReasonDto Reason { get; set; } = new ReasonDto();
        public List<ProductExitDto> productExitDtos { get; set; } = new List<ProductExitDto>();
    }
}
