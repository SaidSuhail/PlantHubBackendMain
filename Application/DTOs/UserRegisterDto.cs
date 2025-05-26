using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
  public  class UserRegisterDto
    {
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get;set; }
        public string? Phone { get; set; }

    }
}
