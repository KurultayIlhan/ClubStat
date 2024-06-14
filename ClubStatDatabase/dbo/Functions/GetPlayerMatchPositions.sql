create function [dbo].[GetPlayerMatchPositions](@matchId int)
returns table 
as return
select 
	PlayerId, 
	Position, 
	OnFieldUtc, 
	OffFieldUtc
from [dbo].[PlayerMatchPositions] p 
where p.MatchId= @matchId;

