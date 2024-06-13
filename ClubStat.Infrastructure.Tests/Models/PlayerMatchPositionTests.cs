using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClubStat.Infrastructure.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure.Models.Tests
{
    [TestClass()]
    public class PlayerMatchPositionTests
    {
        [TestMethod()]
        public void AsJsonTest()
        {
            var player = new PlayerMatchPosition()
            {
                MatchId = 14,
                Position = MatchPosition.LM,
                OffFieldUtc = DateTime.Now.AddMinutes(90),
                OnFieldUtc = DateTime.Now,
                PlayerId = Guid.NewGuid(),
            };
            var json = player.AsJson();
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains(player.PlayerId.ToString(), StringComparison.OrdinalIgnoreCase));

            var copy = json.IsValidJson<PlayerMatchPosition>(PlayerMatchPositionJsonContext.Default.PlayerMatchPosition, out var clone);
            Assert.IsNotNull(clone);
            Assert.IsTrue(copy);
            Assert.AreEqual(player.MatchId, clone.MatchId);
            Assert.AreEqual(player.PlayerId, clone.PlayerId);
            Assert.AreEqual(player.Position, clone.Position);
            Assert.AreEqual(player.OnFieldUtc, clone.OnFieldUtc);
            
            Assert.AreNotEqual(DateTime.MinValue,clone.OnFieldUtc);

            Assert.AreEqual(player.OffFieldUtc, clone.OffFieldUtc);
        }
        [TestMethod]
        public void ListOfPlayerMatchPositionTest()
        {
            var list = new List<PlayerMatchPosition>(12);
            for (var i = 0; i < 12; i++)
            {
                var player = new PlayerMatchPosition()
                {
                    MatchId = 14,
                    Position = (MatchPosition)i,
                    OffFieldUtc = i % 2 == 0 ? DateTime.Now.AddMinutes(45) : (DateTime?)null,//have a left the game for every 2nd player 
                    OnFieldUtc = DateTime.Now,
                    PlayerId = Guid.NewGuid(),
                };
                list.Add(player);
            }

            var json = System.Text.Json.JsonSerializer.Serialize(list, PlayerMatchPositionListJsonContext.Default.ListPlayerMatchPosition);
            Assert.IsFalse(string.IsNullOrEmpty(json));

            //each player is in the json text (serailize success)
            foreach (var entry in list)
            {
                Assert.IsTrue(json.Contains(entry.PlayerId.ToString(), StringComparison.InvariantCultureIgnoreCase), $"Entry {entry.PlayerId} seems to not be json compatible");
            }

            var copy = json.IsValidJson<List<PlayerMatchPosition>>(PlayerMatchPositionListJsonContext.Default.ListPlayerMatchPosition, out var cloned);
            Assert.IsTrue(copy);
            Assert.IsNotNull(cloned);
            Assert.AreEqual(list.Count, cloned.Count, "Expect to have the same number when reversing the json to a list");
            for (var i = 0; i < list.Count; i++)
            {
                var player = list[i];
                var clone = cloned[i];
                Assert.AreEqual(player.MatchId, clone.MatchId);
                Assert.AreEqual(player.PlayerId, clone.PlayerId);
                Assert.AreEqual(player.Position, clone.Position);
                Assert.AreEqual(player.OnFieldUtc, clone.OnFieldUtc);
                Assert.AreEqual(player.OffFieldUtc, clone.OffFieldUtc);
                Assert.AreNotEqual(DateTime.MinValue, clone.OnFieldUtc);
            }
        }


    }
}