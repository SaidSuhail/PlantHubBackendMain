using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
   public interface IUserService
    {
        Task<List<UserViewDto>> GetAllUsers();
        Task<UserViewDto> GetUsersById(int id);
        Task<BlockUnBlockDto> BlockUnBlockUser(int id);
    }
}
