CREATE FUNCTION [dbo].[GetNextMatchForCoach](@coachId uniqueidentifier)
returns table
as return
SELECT        
 M.MatchId
,M.MatchDate 
,M.PlayerLeague 
,M.PlayerLeagueLevel
,m.[AwayTeamGoals]
,m.[HomeTeamGoals]
,a.[ClubId]		 as AwayClubId
,a.[ClubName]	 as AwayClubName
,a.[ClubIconUrl] as AwayClubIconUrl
,a.[ClubCity]	 as AwayCity
,h.[ClubId]		 as HomeClubId
,h.[ClubName]	 as HomeClubName
,h.[ClubIconUrl] as HomeClubIconUrl
,h.[ClubCity]	 as HomeCity
FROM  dbo.[Match] as M 
INNER JOIN dbo.Clubs AS a ON M.AwayTeamClubId = a.ClubId 
INNER JOIN dbo.Clubs AS h ON M.HomeTeamClubId = h.ClubId
INNER JOIN [dbo].[Coaches] AS c ON c.ClubId IN (M.AwayTeamClubId, M.HomeTeamClubId)
WHERE c.[CoachId] = @coachId
and M.[MatchDate] > Getdate();