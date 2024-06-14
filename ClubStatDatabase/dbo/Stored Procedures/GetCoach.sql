create procedure [dbo].[GetCoach]
@userId uniqueidentifier
as
Select 
c.[CoachName] as FullName,
c.[ClubId],
l.[CoachLevel],
l.[PlayersLeague],
l.[PlayersLeagueLevel]
from [dbo].[Coaches] c
join [dbo].[CoachesLeague] as l on l.CoachId= c.CoachId
where c.CoachId= @userId;
