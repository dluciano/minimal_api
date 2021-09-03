CREATE USER PostMinimalApiUser FOR LOGIN [PostMinimalApiLogin];
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].Posts TO PostMinimalApiUser;
GO
GRANT SELECT ON [dbo].[__EFMigrationsHistory] TO PostMinimalApiUser;
GO
GRANT VIEW DEFINITION ON SYMMETRIC KEY::Posts_Key TO PostMinimalApiUser; 
GO
GRANT VIEW DEFINITION ON CERTIFICATE::[PostConntentCertificate] TO PostMinimalApiUser;
GO
GRANT CONTROL ON CERTIFICATE::[PostConntentCertificate] TO PostMinimalApiUser;
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [PostMinimalApiUser];
GO