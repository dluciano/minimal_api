using System.Security.Claims;
using MinimalApi.DataAccess.Context;

namespace MinimalApi.Models;

internal sealed class UserSubProvider : IUserSubProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSubProvider(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public string GetCurrentUserSub()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) throw new Exception("HttpContext is null");
        var userSub = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userSub)) throw new UnauthorizedAccessException("Cannot get userSub");
        return userSub;
    }
}
