CREATE TABLE [dbo].[UserProfilePictures]
(
	[UserId] uniqueidentifier NOT NULL PRIMARY KEY,
	[Png] varbinary(max) not null
)