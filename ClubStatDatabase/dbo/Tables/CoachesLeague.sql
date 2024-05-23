CREATE TABLE [dbo].[CoachesLeague] (
    [CoachId]            UNIQUEIDENTIFIER NOT NULL,
    [PlayersLeague]      INT              NOT NULL,
    [PlayersLeagueLevel] NCHAR (1)        NOT NULL,
    [CoachLevel]         TINYINT          CONSTRAINT [DF_CoachesLeague_CoachLevel] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_CoachesLeague] PRIMARY KEY NONCLUSTERED ([CoachLevel] ASC, [PlayersLeague] ASC, [PlayersLeagueLevel] ASC, [CoachId] ASC),
    CONSTRAINT [FK_CoachesLeague_Coaches] FOREIGN KEY ([CoachId]) REFERENCES [dbo].[Coaches] ([CoachId]) ON DELETE CASCADE
);

