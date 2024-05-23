-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 12/05/2024
-- Description:	Procedure adding or updating a player
-- =============================================
create procedure [dbo].[AddOrUpdatePlayer]
@PlayerName nvarchar(50),
@PlayerDateOfBirth date,
@PlayersLeagueLevel char(1),
@ClubId int,
@BiologicAge int,
@PlayerAttitude int,
@PlayerMotivation int,
@password nvarchar(255) null,
@PlayerId uniqueidentifier null output --optional parameter allowing for adding users
as
BEGIN TRANSACTION


update [dbo].[Players]
SET [PlayerName] = @PlayerName
, [PlayerDateOfBirth]= @PlayerDateOfBirth
, [PlayersLeagueLevel] =@PlayersLeagueLevel
, [ClubId] =@ClubId
, [BiologicAge] = @BiologicAge
, [PlayerAttitude] = @PlayerAttitude
, [PlayerMotivation] =@PlayerMotivation
WHERE [PlayerId] = @PlayerId 
  and @PlayerId is not null;--indicates a new player was added by a coach

if(@@ROWCOUNT=0)
BEGIN
	DECLARE @OutputTbl TABLE (ID UNIQUEIDENTIFIER)
	
	insert into  [dbo].[Players](PlayerName, PlayerDateOfBirth, PlayersLeagueLevel, ClubId, BiologicAge, PlayerAttitude, PlayerMotivation)
	output INSERTED.PlayerId INTO @OutputTbl(ID)
	values(@PlayerName,@PlayerDateOfBirth,@PlayersLeagueLevel,@ClubId,@BiologicAge,@PlayerAttitude,@PlayerMotivation);

	select top(1) @PlayerId = ID from @OutputTbl;
END
COMMIT TRANSACTION

if @password is not null and len(@password)>1 
BEGIN
	exec [dbo].[GenerateLoginFor] @password = @password
								, @UserName = @PlayerName
								, @UserId   = @PlayerId	

END