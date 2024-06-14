create function [dbo].[GetTeam](@clubId int, @age int, @playersLeagueLevel nchar(1))
returns table 
as return(
select * from [dbo].[Players]
where [ClubId]= @clubId
and [PlayersLeagueLevel]=@playersLeagueLevel
and [Age]=@age
)