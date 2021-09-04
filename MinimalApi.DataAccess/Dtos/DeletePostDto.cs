namespace MinimalApi.DataAccess.Dtos
{
    public record DeletePostDto(Guid Id, byte[] RowVersion);
}
