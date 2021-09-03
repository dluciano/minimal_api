CREATE FUNCTION [Security].[tvf_securitypredicate](@UserSub NVARCHAR(255))
    RETURNS TABLE  
    WITH SCHEMABINDING  
AS  
    RETURN SELECT 1 AS [fn_securitypredicate_result]
    WHERE
        DATABASE_PRINCIPAL_ID() = DATABASE_PRINCIPAL_ID('PostMinimalApiUser')
        AND CAST(SESSION_CONTEXT(N'UserSub') AS NVARCHAR(255)) = @UserSub;