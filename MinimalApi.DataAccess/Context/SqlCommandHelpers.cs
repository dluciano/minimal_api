using System.Data.Common;

namespace MinimalApi.DataAccess.Context
{
    internal static class SqlCommandHelpers
    {
        public static DbCommand CreateSetUserSubSessionContext(this DbConnection connection, string userSub)
        {
            var paramName = "@UserSub";

            var cmd = connection.CreateCommand();
            var userSubParameter = cmd.CreateParameter();
            userSubParameter.ParameterName = paramName;
            userSubParameter.Value = userSub;
            cmd.CommandText = @"exec sp_set_session_context @key=N'UserSub', @value=@UserSub";
            cmd.Parameters.Add(userSubParameter);
            return cmd;
        }
    }
}
