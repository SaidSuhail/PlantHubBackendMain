using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
    public interface IUserAddressService
    {
        Task<List<UserAddressDto>> GetUserAddressesAsync(int userId);
        Task<UserAddressDto> AddUserAddressAsync(CreateUserAddressDto dto);
        Task<bool> DeleteAddressAsync(int id);
    }
}
