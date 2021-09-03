namespace MinimalApi.Models.Responses
{
    record PostCreatedResponse(Uri Link, IdResponse Data) : IApiResponse<IdResponse>
    {
        public string Kind { get; } = nameof(PostCreatedResponse);
    }
}