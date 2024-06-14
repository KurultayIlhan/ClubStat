CREATE PROCEDURE [dbo].[SetPlayerMatchPosition]
    @position INT,
    @matchId INT,
    @playerId UNIQUEIDENTIFIER,
    @onField DATETIME2(6),
    @offField DATETIME2(6) = NULL
AS
BEGIN
    SET NOCOUNT OFF;

    BEGIN TRANSACTION;

    -- Use the MERGE statement to insert, update, or delete
    MERGE [dbo].[PlayerMatchPositions] AS target
    USING (SELECT @matchId AS MatchId, @position AS Position, @onField AS OnFieldUtc, @offField AS OffFieldUtc) AS source
    ON (target.MatchId = source.MatchId AND target.Position = source.Position AND target.OnFieldUtc = source.OnFieldUtc)
    WHEN MATCHED AND source.Position = 0 AND source.OffFieldUtc < GETUTCDATE() THEN
        -- If position is 0 and OffField is in the past, delete the row
        DELETE
    WHEN MATCHED AND source.OnFieldUtc > GETUTCDATE() THEN
        -- If the date is in the future, update the existing entry
        UPDATE SET PlayerId = @playerId,
                   OnFieldUtc = @onField,
                   OffFieldUtc = @offField
    WHEN NOT MATCHED THEN
        -- Insert a new entry if no existing entry matches
        INSERT (MatchId, PlayerId, Position, OnFieldUtc, OffFieldUtc)
        VALUES (@matchId, @playerId, @position, @onField, @offField);

   -- Separate delete to delete who is not un the field when the match hasn't started
    DELETE FROM [dbo].[PlayerMatchPositions]
    WHERE MatchId = @matchId 
      AND Position = 0 
      AND OnFieldUtc = @onField
      AND OffFieldUtc < GETUTCDATE();

    COMMIT TRANSACTION;
END

