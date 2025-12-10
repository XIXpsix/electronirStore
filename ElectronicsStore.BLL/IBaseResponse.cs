using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response
{
    public interface IBaseResponse<T>
    {
        // ВАЖНО: Добавлен '?' 
        string? Description { get; set; }

        StatusCode StatusCode { get; set; }

        // ВАЖНО: Добавлен '?'
        T? Data { get; set; }
    }
}