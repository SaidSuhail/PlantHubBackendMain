using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class GoogleLoginDto
    {
        [JsonPropertyName("idToken")] // 👈 important for casing match
        public string IdToken { get; set; }
    }
}
