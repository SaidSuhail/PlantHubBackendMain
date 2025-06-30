using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class SwapBookingResponseDto
    {
        public BookingDto Booking { get; set; }
        public decimal ExtraAmount { get; set; }
    }
}
