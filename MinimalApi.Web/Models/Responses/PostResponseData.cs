namespace MinimalApi.Models.Responses
{
    record PostResponseData(string Title, string? Content, DateTimeOffset CreatedOn, byte[] RowVersion);
}