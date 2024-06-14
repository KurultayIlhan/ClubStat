CREATE PROCEDURE AddPlayerActivity
    @RecordedUtc DATETIME,
    @PlayerId UNIQUEIDENTIFIER,
    @MatchId INT,
    @PlayerClubId INT,
    @Activity INT
AS
BEGIN
    SET NOCOUNT OFF;

    MERGE PlayerActivity AS target
    USING (SELECT @RecordedUtc AS RecordedUtc, @PlayerId AS PlayerId, @MatchId AS MatchId, @PlayerClubId AS PlayerClubId, @Activity AS Activity) AS source
    ON (target.RecordedUtc = source.RecordedUtc
        AND target.PlayerId = source.PlayerId
        AND target.MatchId = source.MatchId
        AND target.Activity = source.Activity)
    WHEN NOT MATCHED THEN
        INSERT (RecordedUtc, PlayerId, MatchId, PlayerClubId, Activity)
        VALUES (source.RecordedUtc, source.PlayerId, source.MatchId, source.PlayerClubId, source.Activity);
END;
