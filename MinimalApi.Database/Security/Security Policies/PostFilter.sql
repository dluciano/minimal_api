CREATE SECURITY POLICY [Security].[PostFilter]
ADD FILTER PREDICATE [Security].[tvf_securitypredicate](CreatedBySub) ON [dbo].[Posts]
WITH (STATE = ON);  
GO