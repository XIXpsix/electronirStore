namespace ElectronicsStore.Domain.Enum
{
    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        UserNotFound = 10,

        // ДОБАВЛЕННЫЕ СТАТУСЫ
        NotFound = 404,
        ProductNotFound = 40,
        // ... (другие статусы, если есть)
    }
}