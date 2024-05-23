
-- =============================================
-- Author:		Ilhan Kurultay
-- Create date: 12/05/2024
-- Description:	Procedure generate or updating a player  credentials
-- =============================================
CREATE procedure [dbo].[GenerateLoginFor]
 @password NVARCHAR(255)
,@UserName NVARCHAR(50) 
,@UserId uniqueidentifier
as

BEGIN TRANSACTION

delete from [dbo].[UsersPrivate] where [UserId]=@UserId;

DECLARE @salt BINARY(16) = CAST(CRYPT_GEN_RANDOM(16) AS BINARY(16));

-- Compute the hash
DECLARE @hashedPassword BINARY(32);
SET @hashedPassword = HASHBYTES('SHA2_256', CONCAT(@salt, CAST(@password AS VARBINARY(510))));

-- Store the hash and the salt
INSERT INTO [dbo].[UsersPrivate](UserId, UserUsername,UserPassword,Salt)
VALUES (@UserId,@UserName, @hashedPassword, @salt);

COMMIT TRANSACTION
return 0;