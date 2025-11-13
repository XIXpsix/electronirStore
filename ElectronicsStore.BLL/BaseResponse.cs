using ElectronicsStore.Domain.Enum;
namespace ElectronicsStore.BLL

{
        public class BaseResponse<T> : IBaseResponse<T>
        {
            // Инициализируем required string, чтобы избежать ошибки инициализатора
            public required string Description { get; set; } = string.Empty;

            // Инициализируем StatusCode
            public StatusCode StatusCode { get; set; } = StatusCode.InternalServerError;

            public T? Data { get; set; }
        }
}