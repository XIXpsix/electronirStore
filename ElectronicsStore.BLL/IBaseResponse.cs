using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.BLL
{
    public interface IBaseResponse<T>
    {
        string Description { get; set; }

        StatusCode StatusCode { get; set; }

        T Data { get; set; }
    }
}