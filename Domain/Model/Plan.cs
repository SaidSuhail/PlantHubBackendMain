using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
   public class Plan
    {
        public int Id { get; set; }

        public string PlanName { get; set; }
        public string Description { get; set; }
        public int MinPlantsAllowed { get; set; }
        public int MaxPlantsAllowed { get; set; }
        public int MonthlyReplacements { get; set; } = 1;
        public int WeeklyServices { get; set; } = 4;
        public decimal Price { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }

    }
}
