CREATE PROCEDURE [dbo].[SavePlayerLocation]
    @PlayerId uniqueidentifier,
    @MatchId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @RecordedUtc datetime2(6)
AS
BEGIN
    BEGIN TRY
        MERGE INTO [dbo].[PlayerLocation] AS target
        USING (SELECT @PlayerId AS PlayerId, @MatchId AS GameId, @RecordedUtc AS RecordedUtc, geography::Point(@Latitude, @Longitude, 4326) AS Location) AS source
        ON (target.PlayerId = source.PlayerId AND target.GameId = source.GameId AND target.RecordedUtc = source.RecordedUtc)
        WHEN MATCHED THEN
            UPDATE SET target.Location = source.Location
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (GameId, PlayerId, RecordedUtc, Location)
            VALUES (source.GameId, source.PlayerId, source.RecordedUtc, source.Location);
    END TRY
    BEGIN CATCH
        -- Handle the error, you can log the error if needed
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
