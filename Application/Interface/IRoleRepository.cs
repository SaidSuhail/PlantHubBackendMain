using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.Model;

namespace Application.Interface
{
   public interface IRoleRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task ChangeUserRoleAsync(User user, UserRole newRole);
    }
}
