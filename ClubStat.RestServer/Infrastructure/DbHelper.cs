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
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;

using Microsoft.Data.SqlClient;

using System.Data;

namespace ClubStat.RestServer.Infrastructure
{
    public class DbHelper
    {
        private readonly ILogger<DbHelper> _logger;
        string _connectionString;

        public DbHelper(IConfiguration config, ILogger<DbHelper> logger)
        {
            this._logger = logger;
            _connectionString = config.GetConnectionString("default") ?? throw new Exception("Can't find the connection string named default in appsettings.json");
        }
        public async Task<LogInResult> LoginUserAsync(LogInUser model)
        {
            try
            {
                using var Con = new SqlConnection(_connectionString);
                using var Cmd = Con.CreateCommand();
                Cmd.CommandText = "[dbo].[CheckPassword]";
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("username", model.Username);
                Cmd.Parameters.AddWithValue("password", model.Password);
                Cmd.Parameters.Add(new SqlParameter("playerId", System.Data.SqlDbType.UniqueIdentifier)
                {
                    Direction = System.Data.ParameterDirection.Output
                });
                Cmd.Parameters.Add(new SqlParameter("userType", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                });
                await Con.OpenAsync().ConfigureAwait(false);
                var result = await Cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                if (Cmd.Parameters[2].Value is Guid userId && Cmd.Parameters[3].Value is int userType)
                {
                    _logger.LogInformation("Login succesfull");
                    return new LogInResult() { UserId = userId, UserType = (UserType)userType };
                }
                else
                {
                    return new LogInResult() { UserType = UserType.None, UserId = Guid.Empty };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process login for user due to a exception");
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
                                    from [dbo].[Players]  where[PlayerId] =@id";
            try
            {
                using var Con = new SqlConnection(_connectionString);
                using var Cmd = Con.CreateCommand();
                Cmd.CommandText = _selectPlayer;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.AddWithValue("id", id);

                await Con.OpenAsync().ConfigureAwait(false);
                using var reader = await Cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!reader.HasRows) { return null; }

                reader.Read();//only 1 row returned so no loop is needed
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

                if (name.Length > 1)//data model does not have first and last name 
                {
                    player.FirstName = name[0];
                    player.LastName = string.Join(' ', name[1..]); // will be bad if double first names are used but will work if double last names 
                }

                return player;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Player data for user due to a exception");
                throw;
            }
        }

        internal async Task<Guid> UpdateAsync(Player model, string? newPassword = null)
        {
            var parameters = new[] {
                new SqlParameter("PlayerName", SqlDbType.NVarChar,50) { Value=model.FullName, Direction= ParameterDirection.Input },
                new SqlParameter("PlayerDateOfBirth", SqlDbType.Date) { Value=model.DateOfBirth.Date, Direction= ParameterDirection.Input  },
                new SqlParameter("PlayersLeagueLevel", SqlDbType.Char,1 ) { Value=model.PlayersLeagueLevel, Direction= ParameterDirection.Input  },
                new SqlParameter("ClubId",model.ClubId),
                new SqlParameter("BiologicAge",model.Age),
                new SqlParameter("PlayerAttitude",model.PlayerAttitude),
                new SqlParameter("PlayerMotivatione",model.PlayerMotivation),
                new SqlParameter("Password",SqlDbType.NVarChar,255){ Direction= ParameterDirection.Input, IsNullable= true,  Value=GetOptionalValue(newPassword) },
                new SqlParameter("PlayerId",SqlDbType.UniqueIdentifier){ Direction= ParameterDirection.InputOutput, IsNullable= true,  Value=GetOptionalValue(model.UserId) },

            };
            using var Con = new SqlConnection(_connectionString);
            using var Cmd = Con.CreateCommand();
            Cmd.CommandText = "AddOrUpdatePlayer";
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddRange(parameters);
            await Con.OpenAsync().ConfigureAwait(false);
            var rows = Cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            _logger.LogDebug("User {name} updated {rows} records", model.FullName, rows);
            //if a new user than remember the user id
            if (Guid.Empty.Equals(model.UserId) && parameters[^1].Value is Guid id)
            {
                model.UserId = id;
            }

            return model.UserId;

        }





        internal async Task<Guid> UpdateAsync(UpdatePassword model)
        {
            if (!model.UserId.HasValue)
            {
                return Guid.Empty;
            }
            using var con = new SqlConnection(_connectionString);
            using var cmd = con.CreateCommand();
            cmd.CommandText = "[dbo].[UpdateLoginFor]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("oldPassword", SqlDbType.NVarChar, 255) { Value = model.CurrentPassword });
            cmd.Parameters.Add(new SqlParameter("Password", SqlDbType.NVarChar, 255) { Value = model.NewPassword });
            cmd.Parameters.Add(new SqlParameter("UserName", SqlDbType.NVarChar, 50) { Value = model.UserName });
            cmd.Parameters.AddWithValue("UserId", model.UserId);
            await con.OpenAsync().ConfigureAwait(false);
            var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (rows == 1)
                return model.UserId.Value;


            return Guid.Empty;

        }


        static object GetOptionalValue(Guid? guid)
        {
            if (!guid.HasValue || Guid.Empty.Equals(guid))
            {
                return DBNull.Value;
            }
            return guid;
        }
        static object GetOptionalValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            return value;
        }
    }


}
