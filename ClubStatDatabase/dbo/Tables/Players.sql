CREATE TABLE [dbo].[Players] (
    [PlayerId]           UNIQUEIDENTIFIER CONSTRAINT [DF_Players_PlayerId] DEFAULT (newsequentialid()) NOT NULL,
    [PlayerName]         NVARCHAR (50)    NOT NULL,
    [PlayerDateOfBirth]  DATE             NOT NULL,
    [PlayerAllowToPlay]  AS               (case when dateadd(year,(-6),getdate())>=CONVERT([date],[PlayerDateOfBirth]) then (1) else (0) end),
    [PlayersLeagueLevel] NCHAR (1)        NOT NULL,
    [ClubId]             INT              NOT NULL,
    [Age]                AS               (datediff(year,[PlayerDateOfBirth],getdate())),
    [BiologicAge]        INT              NULL,
    [PlayerLeague]       AS               (isnull([BiologicAge],datediff(year,[PlayerDateOfBirth],getdate()))),
    [PlayerAttitude]     INT              CONSTRAINT [DF_Players_PlayerAttitude] DEFAULT ((7)) NULL,
    [PlayerMotivation]   INT              CONSTRAINT [DF_Players_PlayerMotivation] DEFAULT ((7)) NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED ([PlayerId] ASC),
    CONSTRAINT [CheckConstraintPlayerName] CHECK (len([PlayerName])>(4))
);






GO
-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 23/04/2024
-- Description:	Foreign key relationship via trigger oplossen.
-- =============================================
CREATE TRIGGER dbo.Fk_Delete_Player_UsersPrivate 
   ON  dbo.Players
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	DELETE FROM dbo.[UsersPrivate] where [UserId] in (select [PlayerId] from deleted)
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Player name must be more than 4 characters.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Players', @level2type = N'CONSTRAINT', @level2name = N'CheckConstraintPlayerName';

