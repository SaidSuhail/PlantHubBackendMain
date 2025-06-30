using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class ReviewRoleChangeRequestDTO
    {
        public int RequestId { get; set; }
        public bool Approve { get; set; }
    }
}
