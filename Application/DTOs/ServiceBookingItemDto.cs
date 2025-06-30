using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class ServiceBookingItemDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
