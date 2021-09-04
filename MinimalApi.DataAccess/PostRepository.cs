using Microsoft.EntityFrameworkCore;
using MinimalApi.DataAccess.Context;
using MinimalApi.DataAccess.Context.Models;
using MinimalApi.DataAccess.Dtos;

namespace MinimalApi.DataAccess
{
    class PostRepository : IPostRepository
    {
        private readonly PostDbContext _postDbContext;

        public PostRepository(PostDbContext postDbContext) =>
            _postDbContext = postDbContext;

        public async Task UpsertAsync(UpsertPostDto createPostDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(createPostDto.UserSub))
                throw new UnauthorizedAccessException();

            var post = new Post(createPostDto.Id,
                createPostDto.Title,
                createPostDto.Content,
                createPostDto.UserSub,
                DateTimeOffset.UtcNow,
                createPostDto.RowVersion);

            var existingPost = await _postDbContext.Posts
                .AsNoTracking()
                .AnyAsync(p => p.PostId == createPostDto.Id, cancellationToken)
                .ConfigureAwait(false);

            if (existingPost)
            {
                _postDbContext.Attach(post);
                _postDbContext.Entry(post).Property(nameof(Post.Title)).IsModified = true;
                _postDbContext.Entry(post).Property(nameof(Post.Content)).IsModified = true;
            }
            else
            {
                await _postDbContext.Posts.AddAsync(post, cancellationToken).ConfigureAwait(false);
            }

            await _postDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<GetAllPostIdsResultDto> GetAllAsync(PageDto pageDto, CancellationToken cancellationToken)
        {
            var postQuery = await _postDbContext.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedOn)
                .Skip(pageDto.Offset)
                .Take(pageDto.Limit)
                .Select(p => new { p.PostId })
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var ids = postQuery.Select(ids => ids.PostId);

            var dto = new GetAllPostIdsResultDto(new(ids));

            return dto;
        }

        public async Task<PostDto?> GetById(Guid id, CancellationToken cancellationToken)
        {
            var post = await _postDbContext.Posts
                .AsNoTracking()
                .Where(p => p.PostId == id)
                .Select(p => new { p.PostId, p.Title, p.Content, p.CreatedOn, p.RowVersion })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (post == default)
            {
                return default;
            }

            var dto = new PostDto(post.PostId, post.Title, post.Content, post.CreatedOn, post.RowVersion);

            return dto;
        }
    }
}
