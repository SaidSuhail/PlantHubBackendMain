using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class ServiceBookingRequestDto
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public string TransactionId { get; set; }
        public List<int> ServiceIds { get; set; }
    }
}
