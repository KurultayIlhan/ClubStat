CREATE PROCEDURE [dbo].[SavePlayerGameEffort]
    @playerId UNIQUEIDENTIFIER,
    @matchId INT,
    @attitude INT,
    @motivation INT
AS
BEGIN
    SET NOCOUNT OFF;
    update [dbo].Players
    set  [PlayerMotivation]= @motivation
        ,[PlayerAttitude]  = @attitude
    where [PlayerId]=@playerId;

    MERGE [dbo].[PlayerGameEffort] AS target
    USING (SELECT @playerId AS PlayerId, @matchId AS GameId, @attitude AS PlayerAttitude, @motivation AS PlayerMotivation) AS source
    ON (target.PlayerId = source.PlayerId AND target.GameId = source.GameId)
    WHEN MATCHED THEN
        UPDATE SET 
            PlayerAttitude = source.PlayerAttitude,
            PlayerMotivation = source.PlayerMotivation
    WHEN NOT MATCHED THEN
        INSERT (PlayerId, GameId, PlayerAttitude, PlayerMotivation)
        VALUES (source.PlayerId, source.GameId, source.PlayerAttitude, source.PlayerMotivation);
END
