CREATE TABLE [dbo].[UsersPrivate] (
    [UserId]          UNIQUEIDENTIFIER NOT NULL,
    [UserPassword]    BINARY (32)      NOT NULL,
    [UserUsername]    NVARCHAR (50)    NOT NULL,
    [UserUsername_CS] AS               (checksum([UserUsername])) PERSISTED,
    [Salt]            BINARY (16)      NOT NULL,
    CONSTRAINT [PK_UsersPrivate] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_UsersPrivate_Coaches] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Coaches] ([CoachId]),
    CONSTRAINT [FK_UsersPrivate_Players] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Players] ([PlayerId])
);


GO
ALTER TABLE [dbo].[UsersPrivate] NOCHECK CONSTRAINT [FK_UsersPrivate_Coaches];


GO
ALTER TABLE [dbo].[UsersPrivate] NOCHECK CONSTRAINT [FK_UsersPrivate_Players];




GO
ALTER TABLE [dbo].[UsersPrivate] NOCHECK CONSTRAINT [FK_UsersPrivate_Coaches];


GO
ALTER TABLE [dbo].[UsersPrivate] NOCHECK CONSTRAINT [FK_UsersPrivate_Players];




GO
ALTER TABLE [dbo].[UsersPrivate] NOCHECK CONSTRAINT [FK_UsersPrivate_Coaches];


GO
ALTER TABLE [dbo].[UsersPrivate] NOCHECK CONSTRAINT [FK_UsersPrivate_Players];


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UsersPrivate_UserName_UserPassword]
    ON [dbo].[UsersPrivate]([UserUsername_CS] ASC, [UserUsername] ASC, [UserPassword] ASC);

