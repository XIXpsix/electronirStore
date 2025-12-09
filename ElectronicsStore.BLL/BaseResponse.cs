using ElectronicsStore.Domain.Enum;


namespace ElectronicsStore.BLL
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string Description { get; set; } = string.Empty;
        public StatusCode StatusCode { get; set; }
        public T? Data { get; set; }
    }
}