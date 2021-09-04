namespace MinimalApi.DataAccess.Dtos
{
    public record PostDto(Guid Id, string Title, string? Content, DateTimeOffset CreatedOn, byte[] RowVersion);
}