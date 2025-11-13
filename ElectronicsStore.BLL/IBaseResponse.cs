using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.BLL
{
    public interface IBaseResponse<T>
    {
        string Description { get; set; }
        StatusCode StatusCode { get; set; }

        // Решение ошибки 5: Также обновляем интерфейс
        T? Data { get; set; }
    }
}