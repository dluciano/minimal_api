CREATE TABLE [Posts] (
    [PostId] uniqueidentifier NOT NULL,
    [Title] nvarchar(512) NOT NULL,
    [Content] nvarchar(max) NULL,
    [CreatedBySub] nvarchar(255) NOT NULL,
    [CreatedOn] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([PostId])
);
GO
