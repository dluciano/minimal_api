namespace MinimalApi.DataAccess.Dtos
{
    public record GetAllPostIdsResultDto(HashSet<Guid> Ids);
}