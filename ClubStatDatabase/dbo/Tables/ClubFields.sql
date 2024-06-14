CREATE TABLE [dbo].[ClubFields] (
    [ClubId]    INT               NOT NULL,
    [FieldNr]   INT               NOT NULL,
    [Location1] [sys].[geography] NULL,
    [Location2] [sys].[geography] NULL,
    [Location3] [sys].[geography] NULL,
    [Location4] [sys].[geography] NULL,
    PRIMARY KEY CLUSTERED ([ClubId] ASC, [FieldNr] ASC)
);

