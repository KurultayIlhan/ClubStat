-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 23/04/2024
-- Description:	Procedure used to login to checking .
-- =============================================
CREATE   PROCEDURE [dbo].[CheckPassword] 
	@password nvarchar(254), 
	@username nvarchar(50),
	@playerId uniqueidentifier output,
	@userType int output
AS
BEGIN 	
	declare @UserUsername_CS int =CHECKSUM(@username);
	Set @userType = 0;
	SELECT Top 1 @playerId = [UserId]
	  FROM [dbo].[UsersPrivate]
	 WHERE @UserUsername_CS = [UserUsername_CS] 	
	   And [UserUsername]	= @username				   
	   AND [UserPassword]	= HASHBYTES('SHA2_256', CONCAT([Salt], CAST(@password AS VARBINARY(MAX))));


	If exists (Select * from dbo.[Players] where [PlayerId] = @playerId) 
		set @userType = 1;
	else If exists (Select * from dbo.[Coaches] where [CoachId] = @playerId)
		set @userType = 2;

END
