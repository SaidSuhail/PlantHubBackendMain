using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Model
{
   public class RoleChangeRequest:BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserRole RequestedRole { get; set; }
        public RoleRequestStatus Status { get; set; } = RoleRequestStatus.Pending;
        public virtual User? User { get; set; }
    }
}
