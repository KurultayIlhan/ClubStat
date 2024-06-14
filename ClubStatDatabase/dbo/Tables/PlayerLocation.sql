CREATE TABLE [dbo].[PlayerLocation]
(
	[GameId] INT NOT NULL ,
	[PlayerId] uniqueidentifier NOT NULL,
	[RecordedUtc] datetime2(6),
	[Location] GEOGRAPHY not null,
	CONSTRAINT [PK_PlayerLocation] PRIMARY KEY CLUSTERED ([GameId],[PlayerId],[RecordedUtc])

)

