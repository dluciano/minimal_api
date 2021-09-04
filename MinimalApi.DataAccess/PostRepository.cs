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

        public async Task<GetAllPostIdsResultDto> GetAllAsync(PageDto pageDto, CancellationToken cancellationToken)
        {
            var postQuery = await _postDbContext.Posts
                .AsNoTracking()
                .Where(p => p.IsActive)
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

        public async Task<PostDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var post = await _postDbContext.Posts
                .AsNoTracking()
                .Where(p => p.PostId == id && p.IsActive)
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

        public async Task<bool> ExistsIdAsync(Guid postId, CancellationToken cancellationToken)
        {
            var exists = await _postDbContext.Posts
                .AsNoTracking()
                .Where(p => p.PostId == postId && p.IsActive)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);
            return exists;
        }

        public async Task UpsertAsync(UpsertPostDto postDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(postDto.UserSub))
                throw new UnauthorizedAccessException();

            var post = new Post(postDto.Id,
                postDto.Title,
                postDto.Content,
                postDto.UserSub,
                DateTimeOffset.UtcNow,
                postDto.RowVersion,
                true);

            var existingPost = await _postDbContext.Posts
                .AsNoTracking()
                .Where(p => p.PostId == postDto.Id)
                .Select(p => new { p.IsActive })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existingPost is not null && !existingPost.IsActive)
                throw new InvalidOperationException("Post was deleted");

            if (existingPost is not null)
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

        public async Task DeleteAsync(DeletePostDto deletePostDto, CancellationToken cancellationToken = default)
        {
            var post = new Post(deletePostDto.Id, string.Empty, string.Empty, string.Empty, default, deletePostDto.RowVersion, false);
            _postDbContext.Attach(post);
            _postDbContext.Entry(post).Property(nameof(Post.IsActive)).IsModified = true;
            await _postDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
