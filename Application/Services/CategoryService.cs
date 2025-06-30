using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
   public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryService(IHttpContextAccessor httpContextAccessor, ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<List<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepo.GetAll();
                var categoryDto =  _mapper.Map<List<CategoryDto>>(categories);
                return new ApiResponse<List<CategoryDto>>(true, "Ctaegories retrieved successfully", categoryDto, null);
            }
            catch(Exception ex)
            {
                return new ApiResponse<List<CategoryDto>>(false, "Failed To Retrieve Category", null, ex.Message);
            }
           
        }
        public async Task<ApiResponse<CategoryAddDto>>AddCategory(CategoryAddDto category)
        {
            var existing = await _categoryRepo.GetByName(category.CategoryName);
            if(existing != null)
            {
                return new ApiResponse<CategoryAddDto>(false, "Category Already exist", null, "Try Another name");
            }
            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";
            
            var entity = _mapper.Map<Category>(category);
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = createdBy;
            await _categoryRepo.Add(entity);
            await _categoryRepo.SaveChanges();
            var result = _mapper.Map<CategoryAddDto>(entity);
            return new ApiResponse<CategoryAddDto>(true, "Category Added", result, null);
        }
        public async Task<ApiResponse<string>>RemoveCategory(int id)
        {
            var category = await _categoryRepo.GetById(id);
            if (category == null)
                return new ApiResponse<string>(false, "Category not found", "", "Invalid ID");
            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";
            var products = await _categoryRepo.GetPlantsByCategoryId(id);
            category.UpdatedAt = DateTime.UtcNow.Date;
            category.UpdatedBy = createdBy;
            _categoryRepo.RemoveProducts(products);
            _categoryRepo.RemoveCategory(category);
            await _categoryRepo.SaveChanges();

            return new ApiResponse<string>(true, "Category deleted", "Done", null);
        }
        public async Task<ApiResponse<bool>> CategoryExistsAsync(int categoryId)
        {
            try
            {
                var exists =  await _categoryRepo.CategoryExistsAsync(categoryId);
                return new ApiResponse<bool>(true, "Category existance checked", exists,null);

            }
            catch(Exception ex)
            {
                return new ApiResponse<bool>(false, "Failed To Check Category Existance", false, ex.Message);
            }
        }

    }
}
