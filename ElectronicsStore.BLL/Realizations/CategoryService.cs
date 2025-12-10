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
    // Использовать основной конструктор: categoryRepository теперь поле класса.
    public class CategoryService(IBaseStorage<Category> categoryRepository) : ICategoryService
    {
        public Task GetAllCategories()
        {
            throw new NotImplementedException();
        }

        // Убран приватный _categoryRepository и явный конструктор.
        // categoryRepository доступен напрямую в методах.

        public async Task<IBaseResponse<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await categoryRepository.GetAll().ToListAsync();

                // выражение new можно упростить (использован BaseResponse, как ваш конкретный тип)
                return new BaseResponse<IEnumerable<Category>>
                {
                    Data = categories,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                // выражение new можно упростить
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