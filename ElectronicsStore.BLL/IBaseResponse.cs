using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response
{
    // Исправлено: T? Data вместо T Data
    public interface IBaseResponse<T>
    {
        StatusCode StatusCode { get; }
        T? Data { get; }
        string Description { get; }
    }
}