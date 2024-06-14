CREATE FUNCTION [dbo].[GetPlayerStatistics](@RecordedAfterUtc DATETIME)
RETURNS TABLE 
AS
RETURN
(
    SELECT 
        PlayerId,
        ISNULL(Assists, 0) AS Assists,
        ISNULL(Goals, 0) AS Goals,
        ISNULL(YellowCards, 0) AS YellowCards,
        ISNULL(RedCards, 0) AS RedCards,
        MinRecordedUtc FromUtc,
        MaxRecordedUtc TillUtc,
        DistinctMatchCount
    FROM 
        (
            SELECT 
                PlayerId,
                MIN(RecordedUtc) AS MinRecordedUtc,
                MAX(RecordedUtc) AS MaxRecordedUtc,
                COUNT(DISTINCT MatchId) AS DistinctMatchCount,
                SUM(CASE WHEN Activity = 1 THEN 1 ELSE 0 END) AS Assists,
                SUM(CASE WHEN Activity = 2 THEN 1 ELSE 0 END) AS Goals,
                SUM(CASE WHEN Activity = 3 THEN 1 ELSE 0 END) AS YellowCards,
                SUM(CASE WHEN Activity = 4 THEN 1 ELSE 0 END) AS RedCards
            FROM 
                PlayerActivity
            WHERE 
                RecordedUtc >= @RecordedAfterUtc
            GROUP BY 
                PlayerId
        ) AS AggregatedData
);
