using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
       public class Plant:BaseEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LatinName { get; set; }

        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string Color { get; set; }

        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int Stock { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Provider? Provider { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; }

    }
}
