using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class CartItemDto
    {
        public Guid Id { get; set; }
        public int PlantId { get; set; }
        public string? PlantName { get; set; }
        public int Quantity { get; set; }
        public string? PlantImage { get; set; }
        public decimal? Price { get; set; }

        public decimal? TotalPrice => Price.HasValue ? Price * Quantity : null;

    }
}
