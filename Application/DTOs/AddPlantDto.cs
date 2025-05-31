using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class AddPlantDto
    {
        [Required]
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        //public string ImageUrl { get; set; }

        public string Color { get; set; }
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int Stock { get; set; }
    }
}
