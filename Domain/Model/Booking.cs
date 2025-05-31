using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
  public  class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public int UserAddressId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? BookingStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string? TransactionId { get; set; }
        public virtual User? User { get; set; }
        public virtual Plan? Plan { get; set; }
        public virtual UserAddress UserAddress { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; }
    }
}
