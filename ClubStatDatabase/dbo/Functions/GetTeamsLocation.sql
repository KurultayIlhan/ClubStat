create function [dbo].[GetTeamsLocation](@clubId INT,@matchId INT)
returns table as return

WITH LatestLocations AS (
    SELECT
        l.[PlayerId],
        MAX(l.[RecordedUtc]) AS LatestRecordedUtc
    FROM [dbo].[PlayerLocation] AS l
    JOIN [dbo].[Players] AS p ON p.[PlayerId] = l.PlayerId
    WHERE p.[ClubId] = @clubId AND l.GameId = @matchId
    GROUP BY l.[PlayerId]
)
SELECT 
    l.[PlayerId],
    CAST(l.[Location].Lat AS NVARCHAR(50)) + ' - ' + CAST(l.[Location].Long AS NVARCHAR(50)) AS [Location],
    l.[RecordedUtc]
FROM [dbo].[PlayerLocation] AS l
JOIN LatestLocations AS ll ON l.[PlayerId] = ll.[PlayerId] AND l.[RecordedUtc] = ll.LatestRecordedUtc
JOIN [dbo].[Players] AS p ON p.[PlayerId] = l.PlayerId
WHERE p.[ClubId] = @clubId AND l.GameId = @matchId;
