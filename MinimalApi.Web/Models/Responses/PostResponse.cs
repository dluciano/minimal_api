namespace MinimalApi.Models.Responses
{
    record PostResponse(Uri Link, PostResponseData Data) : IApiResponse<PostResponseData>
    {
        public string Kind { get; } = nameof(PostResponse);
    }
}