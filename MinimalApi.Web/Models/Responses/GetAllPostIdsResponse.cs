namespace MinimalApi.Models.Responses
{
    record GetAllPostIdsResponse(
        Uri Link,
        HashSet<IdResponse> Data,
        Uri? PreviousPage = default,
        Uri? NextPage = default)
        : IApiResponse<HashSet<IdResponse>>
    {
        public string Kind { get; } = nameof(GetAllPostIdsResponse);
    }
}