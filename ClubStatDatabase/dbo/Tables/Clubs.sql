CREATE TABLE [dbo].[Clubs] (
    [ClubId]      INT           IDENTITY (1, 1) NOT NULL,
    [ClubName]    NVARCHAR (50) NOT NULL,
    [ClubIconUrl] VARCHAR (50)  NOT NULL,
    [ClubCity]    NVARCHAR (50) NULL,
    CONSTRAINT [PK_Clubs] PRIMARY KEY CLUSTERED ([ClubId] ASC)
);




GO
-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 26/04/2024
-- Description:	
-- =============================================
CREATE TRIGGER dbo.trSetCoachesUnkownOnDelete 
   ON  dbo.Clubs 
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	Update [dbo].[Coaches] Set [ClubId] = 0 where [ClubId] in (Select [ClubId] from deleted);

END
