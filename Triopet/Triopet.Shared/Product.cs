using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triopet.Shared.Models;

namespace Triopet.Shared
{
    public class Product
    {
        [Required(ErrorMessage = "Id é obrigatório")]

        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descricao é obrigatório")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Preco é obrigatória")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatório")]
        public string Category { get; set; }

        public List<ImageDto> Images { get; set; } = new();
    }
}