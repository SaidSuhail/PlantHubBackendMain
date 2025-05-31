using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
   public class Provider
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ProviderName { get; set; }
        public string? ContactInfo { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
    }
}
