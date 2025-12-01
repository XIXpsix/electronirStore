using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    // Обязательно public class, реализующий интерфейс
    public class CategoryService : ICategoryService
    {
        private readonly IBaseStorage<Category> _categoryStorage;

        public CategoryService(IBaseStorage<Category> categoryStorage)
        {
            _categoryStorage = categoryStorage;
        }

        public async Task<IBaseResponse<List<Category>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryStorage.GetAll().ToListAsync();

                return new BaseResponse<List<Category>>()
                {
                    Data = categories,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Category>>()
                {
                    Description = $"[GetAllCategories] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}