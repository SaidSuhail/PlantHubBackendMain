using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class UpdatePlanDto
    {
        public string? PlanName { get; set; }
        public string? Description { get; set; }
        public int? MinPlantsAllowed { get; set; }
        public int? MaxPlantsAllowed { get; set; }
        public int? MonthlyReplacements { get; set; }
        public decimal? Price { get; set; }
    }
}
