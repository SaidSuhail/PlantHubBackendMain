using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.DTOs
{
   public class UserViewDto:BaseEntity
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public string? UserEmail { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }

        public bool IsBlocked { get; set; }
        public string? Role { get; set; }
        public string? LoginType { get; set; }
    }
}
