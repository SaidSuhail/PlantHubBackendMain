using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(int id);
        Task<User> BlockUnBlockUser(int id);
        
    }
}
