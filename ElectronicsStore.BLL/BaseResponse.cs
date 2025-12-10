using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;

namespace ElectronicsStore.BLL // Было ElectronicsStore.Domain.Response
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string Description { get; set; }
        public StatusCode StatusCode { get; set; }
        public T Data { get; set; }
    }
}