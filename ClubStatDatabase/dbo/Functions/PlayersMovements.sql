CREATE FUNCTION [dbo].[PlayersMovements](@PlayerId uniqueidentifier, @MatchId int)
RETURNS TABLE
AS
RETURN
(
    WITH PlayerLocationWithPrevious AS (
        SELECT 
            pl.RecordedUtc,
            pl.Location,
            LAG(pl.Location) OVER (PARTITION BY pl.GameId, pl.PlayerId ORDER BY pl.RecordedUtc) AS PreviousLocation,
            LAG(pl.RecordedUtc) OVER (PARTITION BY pl.GameId, pl.PlayerId ORDER BY pl.RecordedUtc) AS PreviousRecordedUtc
        FROM 
            dbo.PlayerLocation pl
        WHERE 
            pl.PlayerId = @PlayerId 
            AND pl.GameId = @MatchId
    )
    SELECT 
        @MatchId as MatchId,
        @PlayerId as PlayerId ,
        pl.RecordedUtc,
        pl.Location,
        CASE 
            WHEN pl.PreviousLocation IS NOT NULL AND pl.PreviousRecordedUtc IS NOT NULL THEN 
                (pl.PreviousLocation.STDistance(pl.Location) / 
                DATEDIFF(SECOND, pl.PreviousRecordedUtc, pl.RecordedUtc)) * 3.6 
            ELSE NULL
        END AS Speed, --To convert meters per second (m/s) to kilometers per hour (km/h), you can use the conversion factor 1m/s=3.6km/h 1m/s=3.6km/h.
        CASE 
            WHEN pl.PreviousLocation IS NOT NULL AND pl.PreviousRecordedUtc IS NOT NULL THEN 
                DEGREES(
                    ATN2(
                        SIN(RADIANS(pl.Location.Long - pl.PreviousLocation.Long)) * COS(RADIANS(pl.Location.Lat)),
                        COS(RADIANS(pl.PreviousLocation.Lat)) * SIN(RADIANS(pl.Location.Lat)) - 
                        SIN(RADIANS(pl.PreviousLocation.Lat)) * COS(RADIANS(pl.Location.Lat)) * 
                        COS(RADIANS(pl.Location.Long - pl.PreviousLocation.Long))
                    )
                )
            ELSE NULL
        END AS Direction
    FROM 
        PlayerLocationWithPrevious pl
);
