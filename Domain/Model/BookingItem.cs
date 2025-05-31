using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
   public class BookingItem
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int PlantId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string PlantImage { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual Plant Plant { get; set; }
    }
}
