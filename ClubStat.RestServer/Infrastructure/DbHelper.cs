// ***********************************************************************
// Assembly         : ClubStat.RestServer
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 14-May-2024
// ***********************************************************************
// <copyright file="DbHelper.cs" company="ClubStat.RestServer">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary>
// Database interaction for the ClubStat project
// </summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;

using Microsoft.Data.SqlClient;

using System.Data;
using System.Runtime.CompilerServices;

using Walter;

namespace ClubStat.RestServer.Infrastructure
{
    public class DbHelper : IDisposable
    {
        private readonly ILogger<DbHelper> _logger;
        SqlConnection _con;
        private bool _disposedValue;

        public DbHelper(IConfiguration config, ILogger<DbHelper> logger)
        {
            this._logger = logger;
            
            var connectionString = config.GetConnectionString("default") ?? throw new Exception("Can't find the connection string named default in appsettings.json");
            _con = new SqlConnection(connectionString);
            _con.InfoMessage += Connection_InfoMessage;


        }
        public async Task<LogInResult> LoginUserAsync(LogInUser model)
        {
            Guard.EnsureNotNullOrEmpty(model?.Username, nameof(model.Username));
            Guard.EnsureNotNullOrEmpty(model?.Password, nameof(model.Password));

            _logger.LogDebug("Processing request login for user {loginName}", model?.Username);
            try
            {

                using var Cmd = _con.CreateCommand();
                Cmd.CommandText = "[dbo].[CheckPassword]";
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("username", model!.Username);
                Cmd.Parameters.AddWithValue("password", model.Password);
                Cmd.Parameters.Add(new SqlParameter("playerId", System.Data.SqlDbType.UniqueIdentifier)
                {
                    Direction = System.Data.ParameterDirection.Output
                });
                Cmd.Parameters.Add(new SqlParameter("userType", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }

                var result = await Cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (Cmd.Parameters[2].Value is Guid userId && Cmd.Parameters[3].Value is int userType)
                {
                    _logger.LogInformation("Login successful");
                    return new LogInResult() { UserId = userId, UserType = (UserType)userType };
                }
                else
                {
                    return new LogInResult() { UserType = UserType.None, UserId = Guid.Empty };
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process login for user due to an exception");
                throw;
            }
        }

        public async Task<Player?> GetPlayerAsync(Guid id)
        {
            const string _selectPlayer = @"select top(1) PlayerName --0
                                      , PlayerDateOfBirth     --1
                                      , PlayersLeagueLevel    --2
                                      , ClubId                --3
                                      , BiologicAge           --4
                                      , PlayerAttitude        --5
                                      , PlayerMotivation      --6
                                      , PlayerLeague          --7
                              from [dbo].[Players]  where [PlayerId] =@id";
            try
            {
                using var Cmd = _con.CreateCommand();
                Cmd.CommandText = _selectPlayer;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.AddWithValue("id", id);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var reader = await Cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!reader.HasRows) { return null; }

                reader.Read(); // only 1 row returned so no loop is needed
                Player player = GetPlayerFromReader(id, reader);

                return player;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL exception occurred while fetching player data for PlayerId: {PlayerId} on line{line} due to:{message}", id, sqlEx.LineNumber, sqlEx.Message);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Player data for PlayerId: {PlayerId} due to an exception", id);
                throw;
            }
        }
        public async Task<List<PlayerMatchPosition>> GetPlayerMatchPositionsAsync(int matchId)
        {
            const string tsql = @"Select 	
        PlayerId, 
        Position, 
        OnFieldUtc, 
        OffFieldUtc 
    from [dbo].[GetPlayerMatchPositions](@MatchId) 
    order by OnFieldUtc;";

            var answer = new List<PlayerMatchPosition>();
            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = tsql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("MatchId", matchId);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!reader.HasRows) { return answer; }

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var pos = new PlayerMatchPosition
                    {
                        MatchId = matchId,
                        PlayerId = reader.GetGuid(reader.GetOrdinal("PlayerId")),
                        Position = (MatchPosition)reader.GetInt32(reader.GetOrdinal("Position")),
                        OnFieldUtc = reader.GetDateTime(reader.GetOrdinal("OnFieldUtc")),
                        OffFieldUtc = reader.IsDBNull(reader.GetOrdinal("OffFieldUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("OffFieldUtc"))
                    };
                    answer.Add(pos);
                }

                return answer;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL exception occurred while fetching player match positions for MatchId: {MatchId} on line:{LineNumber} due to:{message}", matchId, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process player match positions for MatchId: {MatchId} due to an exception", matchId);
                throw;
            }
        }

        private Player GetPlayerFromReader(Guid id, SqlDataReader reader)
        {
            try
            {
                var player = new Player
                {
                    ClubId = reader.GetInt32(3),
                    DateOfBirth = reader.GetDateTime(1),
                    FullName = reader.GetString(0),
                    PlayerAttitude = reader.GetInt32(5),
                    PlayerMotivation = reader.GetInt32(6),
                    PlayersLeague = reader.GetInt32(7),
                    PlayersLeagueLevel = reader.GetString(2)[0],
                    UserId = id,
                    UserType = UserType.Player
                };

                if (reader.GetValue(4) is int biologicalAge)
                {
                    player.BiologicAge = biologicalAge;
                }

                var name = player.FullName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (name.Length > 1) // data model does not have first and last name 
                {
                    player.FirstName = name[0];
                    player.LastName = string.Join(' ', name[1..]); // will be bad if double first names are used but will work if double last names 
                }

                return player;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading player data from SqlDataReader for PlayerId: {PlayerId}", id);
                throw;
            }
        }


        internal async Task<Guid> UpdateAsync(Player model, string? newPassword = null)
        {
            var parameters = new[] {
                new SqlParameter("PlayerName", SqlDbType.NVarChar, 50) { Value = model.FullName, Direction = ParameterDirection.Input },
                new SqlParameter("PlayerDateOfBirth", SqlDbType.Date) { Value = model.DateOfBirth.Date, Direction = ParameterDirection.Input },
                new SqlParameter("PlayersLeagueLevel", SqlDbType.Char, 1) { Value = model.PlayersLeagueLevel, Direction = ParameterDirection.Input },
                new SqlParameter("ClubId", model.ClubId),
                new SqlParameter("BiologicAge", model.Age),
                new SqlParameter("PlayerAttitude", model.PlayerAttitude),
                new SqlParameter("PlayerMotivatione", model.PlayerMotivation),
                new SqlParameter("Password", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Input, IsNullable = true, Value = GetOptionalValue(newPassword) },
                new SqlParameter("PlayerId", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.InputOutput, IsNullable = true, Value = GetOptionalValue(model.UserId) },
            };

            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = "AddOrUpdatePlayer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                _logger.LogDebug("User {name} updated {rows} records", model.FullName, rows);
                // if a new user then remember the user id
                if (Guid.Empty.Equals(model.UserId) && parameters[^1].Value is Guid id)
                {
                    model.UserId = id;
                }

                return model.UserId;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update player data for PlayerId: {PlayerId} due to an exception", model.UserId);
                throw;
            }
        }



        internal async Task<Guid> UpdateAsync(UpdatePassword model)
        {
            if (!model.UserId.HasValue)
            {
                return Guid.Empty;
            }

            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = "[dbo].[UpdateLoginFor]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("oldPassword", SqlDbType.NVarChar, 255) { Value = model.CurrentPassword });
                cmd.Parameters.Add(new SqlParameter("Password", SqlDbType.NVarChar, 255) { Value = model.NewPassword });
                cmd.Parameters.Add(new SqlParameter("UserName", SqlDbType.NVarChar, 50) { Value = model.UserName });
                cmd.Parameters.AddWithValue("UserId", model.UserId);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (rows == 1)
                {
                    return model.UserId.Value;
                }

                return Guid.Empty;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update password for UserId: {UserId} due to an exception", model.UserId);
                throw;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static object GetOptionalValue(Guid? guid)
        {

            if (!guid.HasValue || Guid.Empty.Equals(guid))
            {
                return DBNull.Value;
            }
            return guid;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static object GetOptionalValue(string? value)
        {

            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            return value;

        }


        internal async Task<Match?> GetNextMatchForPlayerAsync(Guid playerId)
        {
            if (Guid.Empty.Equals(playerId)) { return null; }

            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = "select top(1)  * from [dbo].GetNextMatchesForPlayer(@playerId) order by [MatchDate];";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("playerId", playerId);
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                if (!reader.HasRows) { return null; }

                await reader.ReadAsync().ConfigureAwait(false);
                Match match = GetMatchFromReader(reader);

                return match;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL exception occurred while fetching next match for PlayerId: {PlayerId} from GetNextMatchesForPlayer() on line:{LineNumber}", playerId, sqlEx.LineNumber);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch next match for PlayerId: {PlayerId} due to an exception", playerId);
                throw;
            }
        }


        public async Task<List<Match>> GetPlayerAgendaAsync(Guid playerId)
        {
            var result = new List<Match>();

            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using (var cmd = new SqlCommand("select top(4) * from [dbo].[GetPreviousMatchesForPlayer](@playerId) order by MatchDate desc;", _con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("PlayerId", playerId);
                    using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            Match match = GetMatchFromReader(reader);
                            result.Add(match);
                        }
                    }
                }

                using (var cmd = new SqlCommand("select top(2) * from [dbo].[GetNextMatchesForPlayer](@playerId) order by MatchDate desc;", _con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("PlayerId", playerId);
                    using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            Match match = GetMatchFromReader(reader);
                            result.Add(match);
                        }
                    }
                }

                return result;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql}  GetNextMatchesForPlayer for PlayerId: {PlayerId} on line:{line} due to:{message}"
                        , error.Source, playerId, error.LineNumber, error.Message);
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch player agenda for PlayerId: {PlayerId} due to an exception", playerId);
                throw;
            }
        }

        public async Task<Match?> GetPreviousMatchesForPlayerAsync(Guid playerId)
        {
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = new SqlCommand("select top(1) * from [dbo].[GetPreviousMatchesForPlayer](@playerId) order by MatchDate desc;", _con)
                {
                    CommandType = CommandType.Text
                };

                cmd.Parameters.AddWithValue("PlayerId", playerId);
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                if (!reader.HasRows) return null;

                await reader.ReadAsync().ConfigureAwait(false);
                Match match = GetMatchFromReader(reader);
                return match;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql} on GetPreviousMatchesForPlayer PlayerId: {PlayerId} on line:{line} due to:{message}"
                        , error.Source, playerId, error.LineNumber, error.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch previous matches for PlayerId: {PlayerId} due to an exception", playerId);
                throw;
            }
        }

        public async Task<Match?> GetPreviouseMatchForCoachAsync(Guid coachId)
        {
            if (Guid.Empty.Equals(coachId)) { return null; }

            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = "select top(1) * from [dbo].GetPreviouseMatchForCoach(@coachId) order by [MatchDate] desc;";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("coachId", coachId);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                if (!reader.HasRows) { return null; }

                await reader.ReadAsync().ConfigureAwait(false);
                Match match = GetMatchFromReader(reader);

                return match;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql} for coach {coachId} on line:{line} due to:{message}"
                        , error.Source, coachId, error.LineNumber, error.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch previous match for CoachId: {CoachId} due to an exception", coachId);
                throw;
            }
        }

        public async Task<Match?> GetNextMatchForCoachAsync(Guid coachId)
        {
            if (Guid.Empty.Equals(coachId)) { return null; }

            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = "select top(1) * from [dbo].GetNextMatchForCoach(@coachId) order by [MatchDate];";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("coachId", coachId);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                if (!reader.HasRows) { return null; }

                await reader.ReadAsync().ConfigureAwait(false);
                Match match = GetMatchFromReader(reader);

                return match;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql} Next match for Coach: {coachId} on line:{line} due to:{message}"
                        , error.Procedure, coachId, error.LineNumber, error.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch next match for CoachId: {CoachId} due to an exception", coachId);
                throw;
            }
        }


        internal async Task<byte[]> GetProfileImageForAsync(Guid userId)
        {
            try
            {

                using var cmd = _con.CreateCommand();
                cmd.CommandText = "select top(1) [Png] from [dbo].[UserProfilePictures] where UserId=@UserId";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("UserId", userId);
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                var answer = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (answer is byte[] image)
                {
                    return image;
                }
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql} Image for User: {userId} on line:{line} due to:{message}"
                        , error.Procedure, userId, error.LineNumber, error.Message);
                }
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return Array.Empty<byte>();
        }


        internal async Task<bool> UpdateProfileImageForAsync(Guid userId, byte[]? imageBytes)
        {
            if (imageBytes is null || imageBytes.Length == 0) return false;

            const string query = @"
MERGE [dbo].[UserProfilePictures] AS target
USING (VALUES (@UserId, @Png)) AS source ([UserId], [Png])
ON target.[UserId] = source.[UserId]
WHEN MATCHED THEN
    UPDATE SET target.[Png] = source.[Png]
WHEN NOT MATCHED THEN
    INSERT ([UserId], [Png])
    VALUES (source.[UserId], source.[Png]);";

            try
            {

                using var cmd = new SqlCommand(query, _con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Png", imageBytes);

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1; // 1 row affected for update or insert
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql} to retrieve picture for PlayerId: {userId} on line:{LineNumber} due to:{message}"
                        , error.Source, userId, error.LineNumber, error.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogException(ex);

            }

            return false;
        }


        public async Task<List<PlayerMovement>> GetPlayerMovementsAsync(Guid playerId, int matchId)
        {
            var playerMovements = new List<PlayerMovement>();

            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }

                using var command = new SqlCommand("dbo.PlayersMovements", _con);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                command.Parameters.AddWithValue("@MatchId", matchId);
                command.CommandType = CommandType.StoredProcedure;

                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var playerMovement = new PlayerMovement
                        {
                            MatchId = matchId,
                            PlayerId = playerId,
                            Date = reader.GetDateTime(reader.GetOrdinal("RecordedUtc")).Ticks,
                            Location = reader.GetString(reader.GetOrdinal("Location")),
                            Speed = reader.IsDBNull(reader.GetOrdinal("Speed")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Speed")),
                            Direction = reader.IsDBNull(reader.GetOrdinal("Direction")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Direction"))
                        };

                        playerMovements.Add(playerMovement);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }

            return playerMovements;
        }


        public async Task<bool> SavePlayerRecording(Guid playerId, int matchId, double Latitude, double longitude, DateTime recorded)
        {

            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = new SqlCommand("[dbo].[SavePlayerLocation]", _con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("playerId", playerId);
                cmd.Parameters.AddWithValue("matchId", matchId);
                cmd.Parameters.AddWithValue("Latitude", Latitude);
                cmd.Parameters.AddWithValue("longitude", longitude);
                cmd.Parameters.AddWithValue("recordedUtc", recorded);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                _logger.LogTrace("Executing [dbo].[SavePlayerLocation] updated {rows}, expect 1", rows);
                return rows != 0;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}"
                    , sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }


        }


        internal async Task<bool> SavePlayerMotivation(Guid playerId, int matchId, int playerMotivation, int playerAttitude)
        {
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = new SqlCommand("[dbo].[SavePlayerGameEffort]", _con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("playerId", playerId);
                cmd.Parameters.AddWithValue("matchId", matchId);
                cmd.Parameters.AddWithValue("attitude", playerAttitude);
                cmd.Parameters.AddWithValue("motivation", playerMotivation);
                var rows= await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);                
                return rows != 0;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return false;
        }

        internal async Task<(int PlayerAttitude, int PlayerMotivation)> GetPlayerMotivationAsync(Guid playerId, int matchId)
        {
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = new SqlCommand("SELECT PlayerAttitude, PlayerMotivation FROM [dbo].[PlayerGameEffort] WHERE PlayerId = @playerId AND GameId = @matchId", _con);
                cmd.Parameters.AddWithValue("playerId", playerId);
                cmd.Parameters.AddWithValue("matchId", matchId);

                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int playerAttitude = reader.GetInt32(reader.GetOrdinal("PlayerAttitude"));
                    int playerMotivation = reader.GetInt32(reader.GetOrdinal("PlayerMotivation"));
                    return (playerAttitude, playerMotivation);
                }
                else
                {
                    return (0, 0);
                }
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL exception occurred while executing {sql} player agenda for PlayerId: {PlayerId} on line:{line} due to:{message}", error.Procedure, playerId, error.LineNumber, error.Message);
                }
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return (0, 0);
        }


        public async Task<bool> SavePlayerRecordActivityAsync(Guid playerId, int clubId, int matchId, PlayerActivities activity, DateTime recordedUtc)
        {

            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }

                using var cmd = new SqlCommand("[dbo].[AddPlayerActivity]", _con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("RecordedUtc", recordedUtc);
                cmd.Parameters.AddWithValue("PlayerId", playerId);
                cmd.Parameters.AddWithValue("MatchId", matchId);
                cmd.Parameters.AddWithValue("PlayerClubId", clubId);
                cmd.Parameters.AddWithValue("Activity", (int)activity);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (rows != 1)
                    _logger.LogWarning("We expect 1 row for the [dbo].[AddPlayerActivity] procedure, however {rows} returned", rows);

                return rows == 1;

            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }

        }

        private void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            if (e.Errors is not null && e.Errors.Count > 0)
            {
                foreach (SqlError error in e.Errors)
                {

                    _logger.LogError("{Procedure} generated error {Number} on line {line}: {message}"
                        , error.Procedure
                        , error.Number
                        , error.LineNumber
                        , error.Message);
                }
            }

            _logger.LogDebug("{source}: {message}", e.Source, e.Message);
        }

        public async Task<PlayerStatistics> GetPlayerActivityStatistics(Guid playerId, DateTime afterDateUtc)
        {
            var result = new PlayerStatistics();

            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }

                using var cmd = new SqlCommand("SELECT top(1) * from [dbo].[GetPlayerStatistics](@RecordedAfterUtc) where [PlayerId]=@PlayerId", _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("PlayerId", playerId);
                cmd.Parameters.AddWithValue("RecordedAfterUtc", afterDateUtc);
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!reader.HasRows)
                {
                    return result;
                }
                try
                {
                    reader.Read(); // read the single row
                    result.Assists = reader.GetInt32(reader.GetOrdinal("Assists"));
                    result.Goals = reader.GetInt32(reader.GetOrdinal("Goals"));
                    result.Yellow = reader.GetInt32(reader.GetOrdinal("YellowCards"));
                    result.Red = reader.GetInt32(reader.GetOrdinal("RedCards"));
                    result.FromUtc = reader.GetDateTime(reader.GetOrdinal("FromUtc"));
                    result.TillUtc = reader.GetDateTime(reader.GetOrdinal("TillUtc"));
                }
                catch (SqlException sqlEx)
                {
                    foreach (SqlError error in sqlEx.Errors)
                    {
                        _logger.LogError("SQL exception occurred while executing {sql} player agenda for PlayerId: {PlayerId} on line:{line} due to:{message}", error.Procedure, playerId, error.LineNumber, error.Message);
                    }
                    // Handle or rethrow SqlException if needed
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return result;

        }

        public async Task<PlayerMotionStatistics> GetPlayerMotionStatisticsAsync(Guid playerId, int matchId)
        {
            var answer = new PlayerMotionStatistics()
            {
                MatchId = matchId,
                PlayerId = playerId,
                AverageSpeed = 0,
                MedianSpeed = 0,
                Sprints = 0,
                TopSpeed = 0
            };

            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = new SqlCommand("[dbo].[GetPlayerMotionStatistics]", _con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlayerId", playerId);
                cmd.Parameters.AddWithValue("@MatchId", matchId);
                cmd.Parameters.Add(new SqlParameter("@NumberOfSprints", SqlDbType.Int) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@MedianSpeed", SqlDbType.Float) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@TopSpeed", SqlDbType.Float) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@AverageSpeed", SqlDbType.Float) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (cmd.Parameters["@NumberOfSprints"].Value != DBNull.Value)
                {
                    answer.Sprints = (int)cmd.Parameters["@NumberOfSprints"].Value;
                }
                if (cmd.Parameters["@MedianSpeed"].Value != DBNull.Value)
                {
                    answer.MedianSpeed = (double)cmd.Parameters["@MedianSpeed"].Value;
                }
                if (cmd.Parameters["@TopSpeed"].Value != DBNull.Value)
                {
                    answer.TopSpeed = (double)cmd.Parameters["@TopSpeed"].Value;
                }
                if (cmd.Parameters["@AverageSpeed"].Value != DBNull.Value)
                {
                    answer.AverageSpeed = (double)cmd.Parameters["@AverageSpeed"].Value;
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return answer;
        }



        private Match GetMatchFromReader(SqlDataReader reader)
        {
            try
            {
                int matchId = reader.GetInt32(reader.GetOrdinal("MatchId"));
                DateTime matchDate = reader.GetDateTime(reader.GetOrdinal("MatchDate"));
                int playerLeague = reader.GetInt32(reader.GetOrdinal("PlayerLeague"));
                string playerLeagueLevel = reader.GetString(reader.GetOrdinal("PlayerLeagueLevel"));
                int awayTeamGoals = reader.GetByte(reader.GetOrdinal("AwayTeamGoals"));
                int homeTeamGoals = reader.GetByte(reader.GetOrdinal("HomeTeamGoals"));

                int awayClubId = reader.GetInt32(reader.GetOrdinal("AwayClubId"));
                string awayClubName = reader.GetString(reader.GetOrdinal("AwayClubName"));
                string awayClubIconUrl = reader.GetString(reader.GetOrdinal("AwayClubIconUrl"));
                string awayCity = reader.GetString(reader.GetOrdinal("AwayCity"));

                int homeClubId = reader.GetInt32(reader.GetOrdinal("HomeClubId"));
                string homeClubName = reader.GetString(reader.GetOrdinal("HomeClubName"));
                string homeClubIconUrl = reader.GetString(reader.GetOrdinal("HomeClubIconUrl"));
                string homeCity = reader.GetString(reader.GetOrdinal("HomeCity"));

                // Create Club instances
                var awayClub = new Club(awayClubId, awayClubName, awayClubIconUrl, awayCity);
                var homeClub = new Club(homeClubId, homeClubName, homeClubIconUrl, homeCity);

                // Create Match instance
                var match = new Match(homeClub
                                    , awayClub
                                    , playerLeague
                                    , playerLeagueLevel
                                    , matchDate //is already stored as UTC
                                    , awayTeamGoals
                                    , homeTeamGoals
                                    , matchId
                                    );
                return match;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }

        }

        public async Task<List<Player>> GetTeamAsync(int clubId, int age, char playersLeagueLevel)
        {
            var result = new List<Player>();
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }

                using var cmd = new SqlCommand(@"select PlayerName --0
                                        , PlayerDateOfBirth--1
                                        , PlayersLeagueLevel--2
                                        , ClubId--3
                                        , BiologicAge--4
                                        , PlayerAttitude--5
                                        , PlayerMotivation--6
                                        , PlayerLeague--7 
                                        , [PlayerId] --8
                                        from [dbo].[GetTeam](@clubId, @age, @playersLeagueLevel)", _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("clubId", clubId);
                cmd.Parameters.AddWithValue("age", age);
                cmd.Parameters.Add(new SqlParameter("playersLeagueLevel", SqlDbType.Char, 1) { Value = playersLeagueLevel });

                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetGuid(8);
                        var player = GetPlayerFromReader(id, reader);//row 0 till 7 may not change
                        result.Add(player);
                    }
                }

            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError(age, "SQL exception occurred while executing {sql} player agenda for PlayerId: {PlayerId} on line:{line} due to:{message}", error.Procedure, age, error.LineNumber, error.Message);
                }
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // Handle or rethrow other exceptions if needed
            }

            return result;
        }

        public async Task<Coach?> GetCoachAsync(Guid userId)
        {
            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = new SqlCommand("[dbo].[GetCoach]", _con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("userId", userId);
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!reader.HasRows) { return null; }

                await reader.ReadAsync().ConfigureAwait(false);
                var coach = new Coach
                {
                    UserId = userId,
                    FullName = reader.GetString(reader.GetOrdinal(nameof(Coach.FullName))),
                    ClubId = reader.GetInt32(reader.GetOrdinal(nameof(Coach.ClubId))),
                    CoachLevel = reader.GetByte(reader.GetOrdinal(nameof(Coach.CoachLevel))),
                    PlayersLeague = reader.GetInt32(reader.GetOrdinal(nameof(Coach.PlayersLeague))),
                    PlayersLeagueLevel = reader.GetString(reader.GetOrdinal(nameof(Coach.PlayersLeagueLevel)))[0]
                };
                return coach;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // Handle or rethrow other exceptions if needed
            }

            return null;
        }

        internal async Task<bool> SetPlayerMatchPositionsAsync(MatchPosition position, int matchId, Guid playerId, DateTime onField, DateTime? offField)
        {
            _logger.LogDebug("Setting player {PlayerId} position {Position} for match {MatchId} on field at {OnField} off field at {OffField}", playerId, position, matchId, onField, offField);
            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = _con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[SetPlayerMatchPosition]";
                cmd.Parameters.AddWithValue("position", (int)position);
                cmd.Parameters.AddWithValue("matchId", matchId);
                cmd.Parameters.AddWithValue("playerId", playerId);
                cmd.Parameters.AddWithValue("onField", onField);
                if (offField.HasValue)
                {
                    cmd.Parameters.AddWithValue("offField", offField.Value);
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("offField", SqlDbType.DateTime) { IsNullable = true, Value = DBNull.Value });
                }
                var rows= await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return rows != 0;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Stored procedure {Procedure} failed on line:{LineNumber} due to:{Message}", sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // Handle or rethrow other exceptions if needed
            }
            return false;
        }

        internal async Task<List<PlayerMovement>> GetTeamLocationAsync(int clubId, int matchId)
        {
            var result = new List<PlayerMovement>(11);
            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = _con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from [dbo].[GetTeamsLocation]";
                cmd.Parameters.AddWithValue("clubId", clubId);
                cmd.Parameters.AddWithValue("matchId", matchId);
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                if (!reader.HasRows) return result;

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var playerMovement = new PlayerMovement
                    {
                        MatchId = matchId,
                        PlayerId = reader.GetGuid(reader.GetOrdinal("PlayerId")),
                        RecordedUtc = reader.GetDateTime(reader.GetOrdinal("RecordedUtc")),
                        Location = reader.GetString(reader.GetOrdinal("Location")),
                        Speed = (double?)null,
                        Direction = (double?)null
                    };

                    result.Add(playerMovement);
                }

                return result;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("TSQL error {message} while retrieving team members for club:{clubId} and match:{matchId} on line {LineNumber}", sqlEx.Message, clubId, matchId, sqlEx.LineNumber);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }
        }

        internal async Task<PlayerMovement?> GetPlayersLastKnownLocation(int matchId, Guid playerId)
        {
            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = _con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
SELECT
    l.[PlayerId],
    CAST(l.[Location].Lat AS NVARCHAR(50)) + ' - ' + CAST(l.[Location].Long AS NVARCHAR(50)) AS [Location],
    l.[RecordedUtc]
FROM [dbo].[PlayerLocation] AS l
WHERE l.[PlayerId] = @playerId AND l.GameId = @matchId
ORDER BY l.[RecordedUtc] DESC
OFFSET 0 ROWS FETCH NEXT 1 ROW ONLY;";

                cmd.Parameters.AddWithValue("playerId", playerId);
                cmd.Parameters.AddWithValue("matchId", matchId);
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!reader.HasRows) return null;

                await reader.ReadAsync().ConfigureAwait(false);

                var location = new PlayerMovement
                {
                    MatchId = matchId,
                    PlayerId = reader.GetGuid(reader.GetOrdinal("PlayerId")),
                    RecordedUtc = reader.GetDateTime(reader.GetOrdinal("RecordedUtc")),
                    Location = reader.GetString(reader.GetOrdinal("Location")),
                    Speed = (double?)null,
                    Direction = (double?)null
                };

                return location;
            }
            catch (SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("TSQL error {message} last location for player:{playerId} and match:{matchId} on line {LineNumber}"
                        , sqlEx.Message, playerId, matchId, sqlEx.LineNumber);
                }
                // Handle or rethrow SqlException if needed
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // Handle or rethrow other exceptions if needed
            }

            // Return null or handle default behavior if an exception occurs
            return null;
        }

        internal async Task<List<PlayerActivityRow>> GetPlayersActivity(Guid playerId, int matchId)
        {
            try
            {

                if (_con.State != ConnectionState.Open)
                {
                    await _con.OpenAsync().ConfigureAwait(false);
                }
                using var cmd = _con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("PlayerId", playerId);
                cmd.Parameters.AddWithValue("MatchId", matchId);
                cmd.CommandText = "select RecordedUtc, PlayerClubId, Activity from [dbo].[PlayerActivity] where [MatchId]=@MatchId and [PlayerId]=@PlayerId;";
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                var result = new List<PlayerActivityRow>();
                if (!reader.HasRows)
                {
                    return result;
                }

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var entry = new PlayerActivityRow()
                    {
                        PlayerId = playerId,
                        MatchId = matchId,
                        Activity = (PlayerActivities)reader.GetInt32(reader.GetOrdinal("Activity")),
                        RecordedUtc = reader.GetDateTime(reader.GetOrdinal("RecordedUtc")),
                        PlayerClubId = reader.GetInt32(reader.GetOrdinal("PlayerClubId")),
                    };
                    result.Add(entry);
                }

                return result;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Getting player activity failed due to {message} on line:{line}", sqlEx.Message, sqlEx.LineNumber);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }

            // Return null or handle default behavior if an exception occurs

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _con.InfoMessage -= Connection_InfoMessage;
                    _con.Dispose();
                }
                
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DbHelper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

