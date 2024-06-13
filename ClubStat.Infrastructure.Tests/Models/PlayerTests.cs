// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Thu 30-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Thu 30-May-2024
// ***********************************************************************
// <copyright file="PlayerTests.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Models.Tests
{
    [TestClass()]
    public class PlayerTests
    {
        [TestMethod()]
        public void AsJsonTest()
        {
            var player = new Player()
            {
                ClubId = 4,
                DateOfBirth = new DateTime(2009, 12, 07),
                FullName = "Ronaldo",
                PlayerAttitude = 7,
                PlayerMotivation = 6,
                PlayersLeague = 15,
                PlayersLeagueLevel = 'A',
                UserId = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6"),
                UserType = UserType.Player
            };

            var json = player.AsJson();
            var clone = json.IsValidJson<Player>(PlayerJsonContext.Default.Player, out var ronaldo);
            Assert.IsTrue(clone);
            Assert.IsNotNull(ronaldo);
            Assert.AreEqual(player.DateOfBirth, ronaldo.DateOfBirth);
            Assert.AreEqual(player.ClubId, ronaldo.ClubId);
            Assert.AreEqual(player.FullName, ronaldo.FullName);
            Assert.AreEqual(player.PlayerAttitude, ronaldo.PlayerAttitude);
            Assert.AreEqual(player.PlayerMotivation, ronaldo.PlayerMotivation);
            Assert.AreEqual(player.PlayersLeague, ronaldo.PlayersLeague);
            Assert.AreEqual(player.PlayersLeague, ronaldo.PlayersLeague);
            Assert.AreEqual(player.UserType, ronaldo.UserType);
            Assert.AreEqual(player.UserId, ronaldo.UserId);
        }
    }
}