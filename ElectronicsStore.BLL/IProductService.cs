using ElectronicsStore.Domain;
namespace ElectronicsStore.BLL
{
    public interface IProductService
    {
        Task<IBaseResponse<IEnumerable<Product>>> GetProducts();
    }
}
