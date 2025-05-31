using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
   public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategories();
        Task<ApiResponse<CategoryAddDto>> AddCategory(CategoryAddDto category);
        Task<ApiResponse<string>> RemoveCategory(int id);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}
