using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
  public  class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
