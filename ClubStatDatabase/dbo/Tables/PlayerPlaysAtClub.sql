CREATE TABLE [dbo].[PlayerPlaysAtClub] (
    [PlayerId]   UNIQUEIDENTIFIER NOT NULL,
    [ClubId]     INT              NOT NULL,
    [YearJoined] INT              NOT NULL,
    [YearLeft]   INT              NULL,
    CONSTRAINT [PK_PlayerPlaysAtClub] PRIMARY KEY CLUSTERED ([ClubId] ASC, [YearJoined] ASC, [PlayerId] ASC),
    CONSTRAINT [FK_PlayerPlaysAtClub_Clubs] FOREIGN KEY ([ClubId]) REFERENCES [dbo].[Clubs] ([ClubId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlayerPlaysAtClub_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players] ([PlayerId]) ON DELETE CASCADE
);

