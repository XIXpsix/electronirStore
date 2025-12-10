namespace ElectronicsStore.Domain.Enum
{
    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        UserNotFound = 10,
        // ... (другие статусы)
        NotFound = 404, // <-- ИСПРАВЛЕНИЕ ОШИБКИ "NotFound"
        // ...
    }
}