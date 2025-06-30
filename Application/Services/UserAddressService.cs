using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Model;

namespace Application.Services
{
   public class UserAddressService:IUserAddressService
    {
        private readonly IUserAddressRepository _repository;
        private readonly IMapper _mapper;

        public UserAddressService(IUserAddressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<UserAddressDto>>GetUserAddressesAsync(int userId)
        {
            var addresses = await _repository.GetUserAddressesAsync(userId);
            return _mapper.Map<List<UserAddressDto>>(addresses);
        }
        public async Task<UserAddressDto> AddUserAddressAsync(CreateUserAddressDto dto)
        {
            var existingAddresses = await _repository.GetUserAddressesAsync(dto.UserId);
            if (existingAddresses.Count >= 3)
                throw new InvalidOperationException("You can only add upto 3 addresses");
            var entity = _mapper.Map<UserAddress>(dto);
            var added = await _repository.AddAsync(entity);
            return _mapper.Map<UserAddressDto>(added);
        }
        public async Task<bool>DeleteAddressAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
