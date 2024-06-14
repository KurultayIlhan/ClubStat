// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Mon 03-Jun-2024
// ***********************************************************************
// <copyright file="DbHelperTests.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClubStat.RestServer.Infrastructure.Tests
{
    [TestClass()]
    public class DbHelperTests
    {
        private static TestContext? _context;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _context = context;
        }

        [TestMethod()]
        public async Task LoginCoachAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var succesFullLogin = new LogInUser() { Username = "Yohan", Password = "Yohan" };
            var result = await db.LoginUserAsync(succesFullLogin);
            Assert.IsTrue(result.UserType == UserType.Coach);
        }
        [TestMethod()]
        public async Task LoginPlayerAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var succesFullLogin = new LogInUser() { Username = "Ronaldo", Password = "Ronaldo" };
            var result = await db.LoginUserAsync(succesFullLogin);
            Assert.IsTrue(result.UserType == UserType.Player);
        }
        [TestMethod()]
        public async Task FailedLoginUserAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var succesFullLogin = new LogInUser() { Username = "Wrong User", Password = "WrongPassword" };
            var result = await db.LoginUserAsync(succesFullLogin);
            Assert.IsTrue(result.UserType == UserType.None);
        }

        [TestMethod()]
        public async Task GetPlayerAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var expect = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6");
            var ronaldo = await db.GetPlayerAsync(expect);
            Assert.IsNotNull(ronaldo);
            Assert.AreEqual(expect, ronaldo.UserId);
            Assert.AreEqual(new DateTime(2009, 12, 07), ronaldo.DateOfBirth.Date);
            Assert.AreEqual("U15A", ronaldo.League, true);
        }

        [TestMethod()]
        public async Task GetPlayerMotionStatisticsAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var playerId = Guid.Parse("6AD92B0D-2007-EF11-8695-A6939DC6BE4A");
            var stats = await db.GetPlayerMotionStatisticsAsync(playerId, matchId: 1);
            Assert.IsNotNull(stats);
            Assert.AreEqual(playerId, stats.PlayerId);
            Assert.AreNotEqual(0, stats.Sprints);
            Assert.AreNotEqual(0, stats.AverageSpeed);
            Assert.AreNotEqual(0, stats.MedianSpeed);
            Assert.AreNotEqual(0, stats.TopSpeed);
            Assert.IsTrue(stats.TopSpeed > stats.AverageSpeed);


        }

        [TestMethod()]
        public async Task GetPreviousMatchesForPlayerAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var ronaldo = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6");
            var match = await db.GetPreviousMatchesForPlayerAsync(ronaldo);
            Assert.IsNotNull(match);
            Assert.IsTrue(match.MatchDate < DateTime.UtcNow);
        }

        [TestMethod()]
        public async void GetPlayerMovementsAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var messie = Guid.Parse("6AD92B0D-2007-EF11-8695-A6939DC6BE4A");
            var dataset = await db.GetPlayerMovementsAsync(messie, 1);
            Assert.IsTrue(dataset.Count > 0);
        }

        [TestMethod()]
        public async void GetCoachAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var coach = await db.GetCoachAsync(Guid.Parse("89B6F6CD-A401-EF11-8687-C8E26574D4F6"));
            Assert.IsNotNull(coach);
            Assert.IsFalse(string.IsNullOrEmpty(coach.FullName));
        }

        [TestMethod()]
        public async Task GetTeamAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            List<Player> notfound = new();
            var knownPlayers = new[]{Guid.Parse("6AD92B0D-2007-EF11-8695-A6939DC6BE4A")
,Guid.Parse("7E9AC32D-D603-EF11-868D-C8E26574D4F6")
,Guid.Parse("93E14140-D603-EF11-868D-C8E26574D4F6")
,Guid.Parse("8028114C-D603-EF11-868D-C8E26574D4F6")
,Guid.Parse("065FC062-D603-EF11-868D-C8E26574D4F6")
,Guid.Parse("70154218-D803-EF11-868D-C8E26574D4F6")
,Guid.Parse("FE4C7070-8807-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("23731C9A-8807-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("A2C194E7-8807-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("2641CB24-8907-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("00FD4542-8907-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("D312465D-8907-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("CF011D69-8907-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("44EC8A85-8907-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("74958097-8907-EF11-8696-E1B0F78ABC2D")
,Guid.Parse("D07082A6-8907-EF11-8696-E1B0F78ABC2D")
                };

            for (var i = 0; i < knownPlayers.Length; i++)
            {
                var player = await db.GetPlayerAsync(knownPlayers[i]);
                Assert.IsNotNull(player, "The data is supposed to have this data");
                var list = await db.GetTeamAsync(player.ClubId, player.PlayersLeague, player.PlayersLeagueLevel);
                _context?.Write($"{i + 1}/{knownPlayers.Length}:{player.FullName} ");
                if (!list.Contains(player))
                {
                    _context?.WriteLine($"MISSING");
                    notfound.Add(player);
                }
                else
                {
                    _context?.WriteLine($"FOUND");
                }



            }

            Assert.AreEqual(0, notfound.Count, $"List not have any data however {notfound.Count} of {knownPlayers.Length} where not found");
        }
    }
}