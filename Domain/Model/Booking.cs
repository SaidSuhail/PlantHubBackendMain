using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Model
{
  public  class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public int UserAddressId { get; set; }
        public DateTime BookingDate { get; set; }
        public  BookingStatus BookingStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string? TransactionId { get; set; }
        public BookingType BookingType { get; set; }
        public virtual User? User { get; set; }
        public virtual Plan? Plan { get; set; }
        public int? ProviderId { get; set; }
        public virtual Provider? AssignedProvider { get; set; }
        public virtual UserAddress UserAddress { get; set; }
        public virtual ICollection<ServiceBookingItem> ServiceBookingItems { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; }
    }
}
