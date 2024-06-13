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
    public class PlayerGameMotivationTests
    {
        [TestMethod()]
        public void PlayerGameMotivationTest()
        {
            var motivation= new PlayerGameMotivation(Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6"), 1,6,6);
            var json= motivation.AsJson();
            Assert.IsFalse(string.IsNullOrEmpty(json));
            var clone = json.IsValidJson(PlayerGameMotivationJsonContext.Default.PlayerGameMotivation, out var copy);
            Assert.IsTrue(clone);
            Assert.IsNotNull(copy);
            Assert.AreEqual(motivation.PlayerAttitude, copy.PlayerAttitude);
            Assert.AreEqual(motivation.PlayerId, copy.PlayerId);
            Assert.AreEqual(motivation.MatchId, copy.MatchId);
            Assert.AreEqual(motivation.PlayerMotivation,copy.PlayerMotivation);
            Assert.IsTrue(motivation.Equals(copy));
        }

    }
}