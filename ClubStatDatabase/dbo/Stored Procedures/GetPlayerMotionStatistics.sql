CREATE PROCEDURE [dbo].[GetPlayerMotionStatistics]
    @PlayerId UNIQUEIDENTIFIER, 
    @MatchId INT,
    @NumberOfSprints INT OUTPUT,
    @MedianSpeed FLOAT OUTPUT,
    @TopSpeed FLOAT OUTPUT,
    @AverageSpeed FLOAT OUTPUT
AS
BEGIN
    SET NOCOUNT OFF;

	if not exists( select * from dbo.PlayerLocation where PlayerId = @PlayerId AND GameId = @MatchId)
	begin
		select	@NumberOfSprints =0,
				@MedianSpeed =0,
				@TopSpeed =0,
				@AverageSpeed =0;
		return;
	end
    DECLARE @Speeds TABLE (RecordedUtc DATETIME2(6), Speed FLOAT);
    DECLARE @RecordedUtc DATETIME2(6);
    DECLARE @Location GEOGRAPHY;
    DECLARE @PreviousLocation GEOGRAPHY;
    DECLARE @PreviousRecordedUtc DATETIME2(6);

    DECLARE @Speed FLOAT;
    DECLARE @MedianSpeedCalc FLOAT;
    DECLARE @TopSpeedCalc FLOAT = 0;
    DECLARE @TotalSpeed FLOAT = 0;
    DECLARE @Count INT = 0;

    -- Cursor to iterate over player location data and calculate speed
    DECLARE location_cursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT 
        RecordedUtc, 
        Location,
        LAG(Location) OVER (PARTITION BY GameId, PlayerId ORDER BY RecordedUtc) AS PreviousLocation,
        LAG(RecordedUtc) OVER (PARTITION BY GameId, PlayerId ORDER BY RecordedUtc) AS PreviousRecordedUtc
    FROM dbo.PlayerLocation
    WHERE PlayerId = @PlayerId AND GameId = @MatchId;

    OPEN location_cursor;

    FETCH NEXT FROM location_cursor INTO @RecordedUtc, @Location, @PreviousLocation, @PreviousRecordedUtc;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @PreviousLocation IS NOT NULL AND @PreviousRecordedUtc IS NOT NULL
        BEGIN
            SET @Speed = (@PreviousLocation.STDistance(@Location) / DATEDIFF(SECOND, @PreviousRecordedUtc, @RecordedUtc)) * 3.6;

            -- Insert speed into the table
            INSERT INTO @Speeds (RecordedUtc, Speed) VALUES (@RecordedUtc, @Speed);

            -- Calculate TopSpeed, TotalSpeed and Count
            IF @Speed > @TopSpeedCalc
                SET @TopSpeedCalc = @Speed;

            SET @TotalSpeed = @TotalSpeed + @Speed;
            SET @Count = @Count + 1;
        END

        FETCH NEXT FROM location_cursor INTO @RecordedUtc, @Location, @PreviousLocation, @PreviousRecordedUtc;
    END

    CLOSE location_cursor;
    DEALLOCATE location_cursor;

    -- Calculate MedianSpeed
    SELECT @MedianSpeedCalc = AVG(Speed)
    FROM (
        SELECT Speed
        FROM @Speeds
        ORDER BY Speed
        OFFSET ((@Count - 1) / 2) ROWS
        FETCH NEXT 2 ROWS ONLY
    ) AS MedianSpeedRows;

    -- Calculate AverageSpeed
    SET @AverageSpeed = @TotalSpeed / @Count;

    -- Calculate NumberOfSprints (Speed > MedianSpeed)
    ;WITH SprintGroups AS (
        SELECT 
            RecordedUtc,
            Speed,
            CASE 
                WHEN Speed > @MedianSpeedCalc THEN 1 
                ELSE 0 
            END AS IsSprint,
            ROW_NUMBER() OVER (ORDER BY RecordedUtc) - 
            ROW_NUMBER() OVER (PARTITION BY CASE WHEN Speed > @MedianSpeedCalc THEN 1 ELSE 0 END ORDER BY RecordedUtc) AS SprintGroup
        FROM @Speeds
    ),
    FilteredSprints AS (
        SELECT DISTINCT SprintGroup
        FROM SprintGroups
        WHERE IsSprint = 1
    )
    SELECT @NumberOfSprints = COUNT(*)
    FROM FilteredSprints;

    -- Set output parameters
    SET @MedianSpeed = @MedianSpeedCalc;
    SET @TopSpeed = @TopSpeedCalc;
END