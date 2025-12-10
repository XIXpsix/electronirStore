using ElectronicsStore.BLL;
using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response // ВЕРНУЛИ ElectronicsStore.Domain.Response
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        // Добавили '?', чтобы разрешить null и убрать ошибку конструктора
        public string? Description { get; set; }

        public StatusCode StatusCode { get; set; }

        // Добавили '?'
        public T? Data { get; set; }
    }
}