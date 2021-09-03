BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210811153303_CreateDataAccess')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210811153303_CreateDataAccess', N'6.0.0-preview.7.21378.4');
END;
GO

COMMIT;
GO