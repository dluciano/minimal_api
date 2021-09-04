using MinimalApi.DataAccess.Dtos;

namespace MinimalApi.DataAccess
{
    public interface IPostRepository
    {
        Task<GetAllPostIdsResultDto> GetAllAsync(PageDto pageDto, CancellationToken cancellationToken = default);
        Task<PostDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsIdAsync(Guid postId, CancellationToken cancellationToken);

        Task UpsertAsync(UpsertPostDto postDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(DeletePostDto deletePostDto, CancellationToken cancellationToken = default);        
    }
}
