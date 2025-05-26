using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<bool> IsUserExist(string email);
        Task AddUser(User user);
        Task<bool> IsUserNameExist(string userName);
        Task<bool> IsPhoneExist(string phone);

    }
}
