CREATE TABLE [dbo].[Coaches] (
    [CoachId]   UNIQUEIDENTIFIER CONSTRAINT [DF_Coaches_CoachId] DEFAULT (newsequentialid()) NOT NULL,
    [CoachName] NVARCHAR (50)    NOT NULL,
    [ClubId]    INT              NOT NULL,
    CONSTRAINT [PK_Coaches] PRIMARY KEY CLUSTERED ([CoachId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Coaches_Club]
    ON [dbo].[Coaches]([ClubId] ASC);


GO
-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 23/04/2024
-- Description:	Foreign key relationship via trigger oplossen.
-- =============================================
CREATE TRIGGER dbo.Fk_Delete_Coaches_UsersPrivate 
   ON  [dbo].[Coaches] 
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	DELETE FROM dbo.[UsersPrivate] where [UserId] in (select [CoachId] from deleted)
END
