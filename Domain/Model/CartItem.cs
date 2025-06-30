using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
   public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; }
        public int PlantId { get; set; }
        public virtual Plant Plant { get; set; }
        public int Quantity { get; set; }
        public bool IsSwap { get; set; } = false;
        public int? SwappedFromBookingId { get; set; }
        public Booking? SwappedFromBooking { get; set; }
        public decimal PriceDifference { get; set; }
    }
}
