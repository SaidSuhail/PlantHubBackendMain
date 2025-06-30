using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
  public  class BookingItemDto
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string PlantImage { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
