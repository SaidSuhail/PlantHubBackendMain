using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
   public class UpdateUserprofileDto
    {
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public IFormFile? ProfileImage { get; set; }

    }
}
