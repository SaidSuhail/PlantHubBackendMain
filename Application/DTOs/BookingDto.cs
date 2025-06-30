using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.DTOs
{
   public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; } // ✅ Add this line

        public int PlanId { get; set; }
        public int UserAddressId { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string TransactionId { get; set; }
        public string BookingType { get; set; }
        public List<BookingItemDto> BookingItems { get; set; }
        public List<ServiceBookingItemDto> ServiceBookingItems { get; set; }
    }
}
