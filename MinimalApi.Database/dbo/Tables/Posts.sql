CREATE TABLE [dbo].[Posts] (
    [PostId]       UNIQUEIDENTIFIER   NOT NULL,
    [Title]        NVARCHAR (512)     NOT NULL,
    [Content]      NVARCHAR (MAX)     NULL,
    [CreatedBySub] NVARCHAR (255)     NOT NULL,
    [CreatedOn]    DATETIMEOFFSET (7) NOT NULL,
    [RowVersion]   ROWVERSION         NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED ([PostId] ASC)
);


GO
