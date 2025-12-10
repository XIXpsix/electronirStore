using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Response
{
    public interface IBaseResponse<T>
    {
        StatusCode StatusCode { get; }
        T? Data { get; }
        string Description { get; }
    }
}