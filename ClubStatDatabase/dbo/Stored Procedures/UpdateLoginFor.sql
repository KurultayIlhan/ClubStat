
-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 12/05/2024
-- Description:	Procedure update password if know old password
-- =============================================
create procedure [dbo].[UpdateLoginFor]
 @password NVARCHAR(255)
,@oldPassword NVARCHAR(255)
,@UserName NVARCHAR(50) 
,@UserId uniqueidentifier
as

BEGIN TRANSACTION

DECLARE @UserUsername_CS int =CHECKSUM(@username),
		@salt BINARY(16) = CAST(CRYPT_GEN_RANDOM(16) AS BINARY(16));

DECLARE	@hashedPassword BINARY(32)= HASHBYTES('SHA2_256', CONCAT(@salt, CAST(@password AS VARBINARY(510))));

	UPDATE [dbo].[UsersPrivate]
		SET [Salt]=@salt
           ,[UserPassword]= @hashedPassword
	 WHERE @UserUsername_CS = [UserUsername_CS] 	
	   And [UserUsername]	= @username				   
	   AND [UserPassword]	= HASHBYTES('SHA2_256', CONCAT([Salt], CAST(@oldPassword AS VARBINARY(MAX))))
	   AND [UserId] = @UserId;


COMMIT TRANSACTION
return 0;