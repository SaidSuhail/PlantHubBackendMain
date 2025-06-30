using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Enum;
using Domain.Model;

namespace Application.Mapper
{
   public class ProfileMapper:Profile
    {
        public ProfileMapper()
        {
            CreateMap<User, UserRegisterDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryAddDto>().ReverseMap();
            CreateMap<Plant, AddPlantDto>().ReverseMap()
            .ForMember(dest => dest.CareLevel, opt => opt.MapFrom(src => src.CareLevel.ToString()));
            CreateMap<Plant, UpdatePlantDto>().ReverseMap();
            CreateMap<Plant, PlantWithCategoryDto>()
                .ForMember(dest=>dest.CategoryName,opt=>opt.MapFrom(src=>src.Category != null?src.Category.CategoryName:null)).ReverseMap();
            CreateMap<Plan, PlanDto>().ReverseMap();
            CreateMap<UserPlan, UserPlanDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            CreateMap<AddUserPlanDto, UserPlan>();
            CreateMap<User, UserViewDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.LoginType, opt => opt.MapFrom(src => src.loginType.ToString()));
            CreateMap<User, UserProfileDto>()
                  .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.ProfileImage))
                  .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<CartItem, CartItemDto>()
                //.ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Plant.Name));
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Plant != null ? src.Plant.Name : null))
                .ForMember(dest => dest.PlantImage, opt => opt.MapFrom(src => src.Plant != null ? src.Plant.ImageUrl : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Plant != null ? src.Plant.Price : 0m));
             CreateMap<Cart, CartWithTotalPriceDto>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));
            CreateMap<Booking, BookingDto>()
               .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.BookingStatus.ToString()))
               .ForMember(dest => dest.BookingType, opt => opt.MapFrom(src => src.BookingType.ToString()))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.AssignedProvider.ProviderName))
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
               .ForMember(dest => dest.ServiceBookingItems, opt => opt.MapFrom(src => src.ServiceBookingItems))
                   .ForMember(dest => dest.BookingItems, opt => opt.MapFrom(src => src.BookingItems));
            CreateMap<BookingItem, BookingItemDto>()
               .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Plant.Name))
               .ForMember(dest => dest.PlantImage, opt => opt.MapFrom(src => src.Plant.ImageUrl));
            CreateMap<UserAddress, UserAddressDto>().ReverseMap();
            CreateMap<UserAddress, CreateUserAddressDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<Service, CreateServiceDto>().ReverseMap();
            CreateMap<Service, UpdateServiceDto>().ReverseMap();
            CreateMap<ServiceBookingItem, ServiceBookingItemDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));
            CreateMap<Booking, SwapBookingResponseDto>();

        }
    }
}
