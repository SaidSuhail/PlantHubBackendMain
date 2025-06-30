using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
   public class ServiceBookingItem
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual Service Service { get; set; }
    }
}
