CREATE TABLE [dbo].[Match] (
    [AwayTeamClubId]    INT           NOT NULL,
    [HomeTeamClubId]    INT           NOT NULL,
    [PlayerLeague]      INT           NOT NULL,
    [PlayerLeagueLevel] NCHAR (1)     NOT NULL,
    [MatchDate]         SMALLDATETIME CONSTRAINT [DF_Match_MatchDate] DEFAULT (getdate()) NOT NULL,
    [AwayTeamGoals]     TINYINT       CONSTRAINT [DF_Match_AwayTeamGoals] DEFAULT ((0)) NOT NULL,
    [HomeTeamGoals]     TINYINT       CONSTRAINT [DF_Match_HomeTeamGoals] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Match] PRIMARY KEY CLUSTERED ([MatchDate] ASC, [PlayerLeague] ASC, [PlayerLeagueLevel] ASC, [HomeTeamClubId] ASC, [AwayTeamClubId] ASC)
);

