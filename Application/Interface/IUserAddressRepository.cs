using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IUserAddressRepository
    {
        Task<List<UserAddress>> GetUserAddressesAsync(int userId);
        Task<UserAddress?> GetByIdAsync(int id);
        Task<UserAddress> AddAsync(UserAddress address);
        Task<bool> DeleteAsync(int id);
    }
}
