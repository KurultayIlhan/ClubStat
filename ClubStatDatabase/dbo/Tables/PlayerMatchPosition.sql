create table [dbo].[PlayerMatchPositions]
(
	[MatchId] int not null,
	[PlayerId] uniqueidentifier not null,
	[Position] int not null,
	[OnFieldUtc] datetime2(6) not null,
	[OffFieldUtc] datetime2(6) null
	primary key([MatchId],[PlayerId],[Position],[OnFieldUtc])
)
