using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class UserAddressDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string HomeAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string PostalCode { get; set; }
    }
}
