namespace MinimalApi.Models.Responses
{
    record PostUpsertedResponse(Uri Link, IdResponse Data) : IApiResponse<IdResponse>
    {
        public string Kind { get; } = nameof(PostUpsertedResponse);
    }
}