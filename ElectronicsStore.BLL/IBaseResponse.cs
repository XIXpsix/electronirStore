using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response // ВЕРНУЛИ ElectronicsStore.Domain.Response
{
    public interface IBaseResponse<T>
    {
        string? Description { get; set; }
        StatusCode StatusCode { get; set; }
        T? Data { get; set; }
    }
}