using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Model
{
     public class User:BaseEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20,MinimumLength =3)]
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public bool IsBlocked { get; set; }
        public UserRole Role { get; set; }
        public  enum LoginType { Local,Google}
        public LoginType loginType { get; set; }
        public virtual ICollection< Provider> Providers { get; set; }
        public virtual ICollection<UserPlan> UserPlans { get; set; }

    }
}
