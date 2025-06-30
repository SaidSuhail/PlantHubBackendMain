using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IProfileRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(User user);
    }
}
