CREATE TABLE [dbo].[PlayerGameEffort] (
    [GameId]      INT               NOT NULL,
    [PlayerId]    UNIQUEIDENTIFIER  NOT NULL,
    [PlayerAttitude]    INT NOT NULL,
	[PlayerMotivation] INT NOT NULL,
    CONSTRAINT [PK_PlayerGameEffort] PRIMARY KEY CLUSTERED ([GameId] ASC, [PlayerId] ASC)
);

