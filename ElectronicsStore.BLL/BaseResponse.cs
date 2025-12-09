using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        // Инициализируем пустой строкой, чтобы убрать ошибку
        public string Description { get; set; } = "";

        public StatusCode StatusCode { get; set; }

        // Разрешаем null для Data
        public T? Data { get; set; }
    }
}