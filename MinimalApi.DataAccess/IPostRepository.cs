using MinimalApi.DataAccess.Dtos;

namespace MinimalApi.DataAccess
{
    public interface IPostRepository
    {
        Task<GetAllPostIdsResultDto> GetAllAsync(PageDto pageDto, CancellationToken cancellationToken = default);
        Task<PostDto?> GetById(Guid id, CancellationToken cancellationToken = default);
        Task UpsertAsync(UpsertPostDto createPostDto, CancellationToken cancellationToken = default);
    }
}
