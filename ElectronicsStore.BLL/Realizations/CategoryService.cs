using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseStorage<Category> _categoryRepository;

        public CategoryService(IBaseStorage<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IBaseResponse<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAll().ToListAsync();
                return new BaseResponse<IEnumerable<Category>>
                {
                    Data = categories,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Category>>
                {
                    Description = $"[GetCategories]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public Task<IBaseResponse<Category>> GetCategoryById(int id)
        {
            throw new NotImplementedException();
        }
    }
}