using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTOs
{
   public class PlanDto
    {
        public int? Id { get; set; }
        public string? PlanName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? MinPlantsAllowed { get; set; }
        public int? MaxPlantsAllowed { get; set; }
        public int? MonthlyReplacements { get; set; }
        public decimal? Price { get; set; }
        //public BillingCycle BillingCycle { get; set; } = BillingCycle.Quarterly;
    }
}
