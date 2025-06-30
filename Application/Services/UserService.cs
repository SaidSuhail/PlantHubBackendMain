using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;

namespace Application.Services
{
   public class UserService:IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<UserViewDto>> GetAllUsers()
        {
            var users = await _repository.GetAllUsers();
            return _mapper.Map<List<UserViewDto>>(users);
        }
        public async Task<UserViewDto> GetUsersById(int id)
        {
            var user = await _repository.GetUserById(id);
            return _mapper.Map<UserViewDto>(user);
        }
        public async  Task<BlockUnBlockDto> BlockUnBlockUser(int id)
        {
            var user = await _repository.BlockUnBlockUser(id);
            return new BlockUnBlockDto
            {
                IsBlocked = user.IsBlocked,
                Msg = user.IsBlocked ? "User Blocked" : "User Unblocked"
            };
        }
    }
}
