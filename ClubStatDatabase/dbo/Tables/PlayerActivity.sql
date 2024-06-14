CREATE TABLE PlayerActivity (
    RecordedUtc DATETIME NOT NULL,
    PlayerId UNIQUEIDENTIFIER NOT NULL,
    MatchId INT NOT NULL,
    PlayerClubId INT NOT NULL,
    Activity INT NOT NULL,
    PRIMARY KEY (RecordedUtc, PlayerId,MatchId,Activity)
);