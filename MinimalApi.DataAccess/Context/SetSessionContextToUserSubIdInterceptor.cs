using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MinimalApi.DataAccess.Context
{
    internal interface ISetSessionContextToUserSubIdInterceptor : IDbConnectionInterceptor { }

    internal sealed class SetSessionContextToUserSubIdInterceptor : DbConnectionInterceptor, ISetSessionContextToUserSubIdInterceptor
    {
        private readonly IUserSubProvider _userSubProvider;

        public SetSessionContextToUserSubIdInterceptor(IUserSubProvider userSubProvider) =>
            _userSubProvider = userSubProvider;

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            var userSub = _userSubProvider.GetCurrentUserSub();
            var cmd = connection.CreateSetUserSubSessionContext(userSub);
            cmd.ExecuteNonQuery();
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            var userSub = _userSubProvider.GetCurrentUserSub();
            if (string.IsNullOrWhiteSpace(userSub)) return;
            var cmd = connection.CreateSetUserSubSessionContext(userSub);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
