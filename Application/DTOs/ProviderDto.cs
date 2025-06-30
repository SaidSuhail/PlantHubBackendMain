using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class ProviderDto
    {
        public int Id { get; set; }             // ProviderId
        public int UserId { get; set; }         // Linked UserId
        public string ProviderName { get; set; }
        public string ContactInfo { get; set; }
        public string UserName { get; set; }    // From User table
        public string UserEmail { get; set; }   // From User table
    }
}
