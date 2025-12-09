using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Domain.Response;


namespace ElectronicsStore.BLL.Realizations
{
    public class CategoryService(IBaseStorage<Category> categoryStorage) : ICategoryService
    {
        public async Task<IBaseResponse<List<Category>>> GetAllCategories()
        {
            try
            {
                var categories = await categoryStorage.GetAll().ToListAsync();

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