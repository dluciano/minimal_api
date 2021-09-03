namespace MinimalApi.Models.Responses
{
    interface IApiResponse<out T>
        where T : class
    {
        Uri Link { get; }
        string Kind { get; }
        T Data { get; }
    }
}