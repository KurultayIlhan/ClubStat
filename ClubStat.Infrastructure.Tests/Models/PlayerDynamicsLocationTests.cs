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
    public class PlayerDynamicsLocationTests
    {
        [TestMethod()]
        public void PlayerDynamicsLocationTest()
        {

            var location = new PlayerDynamicsLocation(
                             lat: 50.833862598105696m
                            ,lng: 4.440449089283627m
                            ,recorded: DateTime.Now
                            ,playerId: Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6")
                            );
            var json= location.AsJson();
            Assert.IsFalse(string.IsNullOrEmpty(json));
            var clone = json.IsValidJson(PlayerDynamicsLocationJsonContext.Default.PlayerDynamicsLocation, out var copy);
            Assert.IsTrue(clone);
            Assert.IsNotNull(copy);
            Assert.AreEqual(location.Lng, copy.Lng);
            Assert.AreEqual(location.Lat, copy.Lat);
            Assert.AreEqual(location.Recorded, copy.Recorded);
            Assert.AreEqual(location.PlayerId, copy.PlayerId);
            
            Assert.AreNotEqual(DateTime.MinValue,copy.Recorded);

            Assert.IsTrue(location.Equals(copy));
        }

            [TestMethod()]

        public void PlayerDynamicsLocationsTest()
        {
            var items= new List<PlayerDynamicsLocation>()
            {
                new PlayerDynamicsLocation(
                             lat: 50.833872m
                            ,lng: 4.44044918m
                            ,recorded: DateTime.Now
                            ,playerId: Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6")
                            ),
                new PlayerDynamicsLocation(
                         lat: 50.8338625m
                        ,lng: 4.44044908m
                        ,recorded: DateTime.Now.AddSeconds(1)
                        ,playerId: Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6")
                        )
                };

            var json = System.Text.Json.JsonSerializer.Serialize(items, PlayerDynamicsLocationsJsonContext.Default.ListPlayerDynamicsLocation);
            Assert.IsFalse(string.IsNullOrEmpty(json));
            var clone = json.IsValidJson(PlayerDynamicsLocationsJsonContext.Default.ListPlayerDynamicsLocation, out var list);
            Assert.IsTrue(clone);
            Assert.IsNotNull(list);
            Assert.AreEqual(items.Count, list.Count);

            for (int i = 0; i < items.Count; i++)
            {
                var copy = list[i];
                var location = items[i];
                Assert.AreEqual(location.Lng, copy.Lng);
                Assert.AreEqual(location.Lat, copy.Lat);
                Assert.AreEqual(location.Recorded, copy.Recorded);
                Assert.AreEqual(location.PlayerId, copy.PlayerId);
                
                Assert.AreNotEqual(DateTime.MinValue, copy.Recorded);

                Assert.IsTrue(location.Equals(copy));
            }
        }

    }
}