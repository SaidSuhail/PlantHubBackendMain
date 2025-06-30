using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class CartWithTotalPriceDto
    {
        public Guid CartId { get; set; }
        public List<CartItemDto>? Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
