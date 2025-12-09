using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response
{
    public interface IBaseResponse<T>
    {
        string Description { get; }
        StatusCode StatusCode { get; }
        T? Data { get; } // Добавили '?', теперь Data может быть null
    }
}