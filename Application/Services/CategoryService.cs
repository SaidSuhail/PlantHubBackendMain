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
   public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        public async Task<List<CategoryDto>> GetCategories()
        {
            var categories = await _categoryRepo.GetAll();
            return _mapper.Map<List<CategoryDto>>(categories);
        }
        public async Task<ApiResponse<CategoryAddDto>>AddCategory(CategoryAddDto category)
        {
            var existing = await _categoryRepo.GetByName(category.CategoryName);
            if(existing != null)
            {
                return new ApiResponse<CategoryAddDto>(false, "Category Already exist", null, "Try Another name");
            }
            var entity = _mapper.Map<Category>(category);
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

            var products = await _categoryRepo.GetPlantsByCategoryId(id);
            _categoryRepo.RemoveProducts(products);
            _categoryRepo.RemoveCategory(category);
            await _categoryRepo.SaveChanges();

            return new ApiResponse<string>(true, "Category deleted", "Done", null);
        }
        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _categoryRepo.CategoryExistsAsync(categoryId);
        }

    }
}
