CREATE PROCEDURE [dbo].[PlayersMovements](@PlayerId UNIQUEIDENTIFIER, @MatchId INT)
AS
BEGIN
    DECLARE @Result TABLE
    (
        MatchId INT,
        PlayerId UNIQUEIDENTIFIER,
        RecordedUtc DATETIME2(6),
        Location GEOGRAPHY,
        Speed FLOAT,
        Direction FLOAT
    );

    -- Declare variables to store intermediate values
    DECLARE @RecordedUtc DATETIME2(6);
    DECLARE @Location GEOGRAPHY;
    DECLARE @PreviousLocation GEOGRAPHY;
    DECLARE @PreviousRecordedUtc DATETIME2(6);
    DECLARE @Distance FLOAT;
    DECLARE @TimeDiff INT;
    DECLARE @Speed FLOAT;
    DECLARE @Direction FLOAT;
    DECLARE @LatDiff FLOAT;
    DECLARE @LonDiff FLOAT;
    DECLARE @LatPrev FLOAT;
    DECLARE @LatCurr FLOAT;
    DECLARE @LonPrev FLOAT;
    DECLARE @LonCurr FLOAT;

    -- Cursor to iterate over player location data
    DECLARE location_cursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT 
        RecordedUtc, 
        Location,
        LAG(Location) OVER (PARTITION BY GameId, PlayerId ORDER BY RecordedUtc) AS PreviousLocation,
        LAG(RecordedUtc) OVER (PARTITION BY GameId, PlayerId ORDER BY RecordedUtc) AS PreviousRecordedUtc
    FROM 
        dbo.PlayerLocation
    WHERE 
        PlayerId = @PlayerId 
        AND GameId = @MatchId;

    OPEN location_cursor;

    FETCH NEXT FROM location_cursor INTO @RecordedUtc, @Location, @PreviousLocation, @PreviousRecordedUtc;
   
   SET @Direction = 0;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Initialize default values
        SET @Speed = NULL;


        IF @PreviousLocation IS NOT NULL AND @PreviousRecordedUtc IS NOT NULL
        BEGIN
            -- Calculate distance
            SET @Distance = @PreviousLocation.STDistance(@Location);
            
            -- Calculate time difference
            SET @TimeDiff = DATEDIFF(SECOND, @PreviousRecordedUtc, @RecordedUtc);
            
            IF @TimeDiff > 0 AND @Distance IS NOT NULL
            BEGIN
                -- Calculate speed
                SET @Speed = (@Distance / @TimeDiff) * 3.6;
            END

            -- Calculate direction
            SELECT 
                @LatPrev = @PreviousLocation.Lat,
                @LonPrev = @PreviousLocation.Long,
                @LatCurr = @Location.Lat,
                @LonCurr = @Location.Long;

            IF @LatPrev IS NOT NULL AND @LonPrev IS NOT NULL AND @LatCurr IS NOT NULL AND @LonCurr IS NOT NULL
                AND @LatPrev BETWEEN -90 AND 90
                AND @LatCurr BETWEEN -90 AND 90
                AND @LonPrev BETWEEN -180 AND 180
                AND @LonCurr BETWEEN -180 AND 180
            BEGIN
                SET @LatDiff = RADIANS(@LatCurr - @LatPrev);
                SET @LonDiff = RADIANS(@LonCurr - @LonPrev);

                -- Ensure the differences are not too small to cause precision issues
                IF ABS(@LatDiff) > 0.000001 OR ABS(@LonDiff) > 0.000001
                BEGIN
                    SET @Direction = DEGREES(
                        ATN2(
                            SIN(@LonDiff) * COS(RADIANS(@LatCurr)),
                            COS(RADIANS(@LatPrev)) * SIN(RADIANS(@LatCurr)) - 
                            SIN(RADIANS(@LatPrev)) * COS(RADIANS(@LatCurr)) * COS(@LonDiff)
                        )
                    );
                END
            END
        END

        -- Insert into result table
        INSERT INTO @Result (MatchId, PlayerId, RecordedUtc, Location, Speed, Direction)
        VALUES (@MatchId, @PlayerId, @RecordedUtc, @Location, @Speed, @Direction);

        FETCH NEXT FROM location_cursor INTO @RecordedUtc, @Location, @PreviousLocation, @PreviousRecordedUtc;
    END

    CLOSE location_cursor;
    DEALLOCATE location_cursor;

SELECT 
    MatchId,
    PlayerId,
    RecordedUtc,
     CAST(Location.Lat AS NVARCHAR(50)) + ' - ' + CAST(Location.Long AS NVARCHAR(50)) AS [Location], 
    Speed,
    Direction
FROM @Result;

END
