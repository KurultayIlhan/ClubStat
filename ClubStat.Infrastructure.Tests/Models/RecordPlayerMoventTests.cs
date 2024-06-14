// ***********************************************************************
// Assembly         : lubStat.Infrastructure.Tests
// Author           : Ilhan
// Created          : Fri 31-May-2024
//
// Last Modified By : Walter Verhoeven
// Last Modified On : Fri 07-Jun-2024
// ***********************************************************************
// <copyright file="RecordPlayerMoventTests.cs" company="Ilhan">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Models.Tests
{
    [TestClass()]
    public class RecordPlayerMoventTests
    {
        [TestMethod()]
        public void RecordPlayerMoventTest()
        {
            var date = DateTime.UtcNow;
            var id = Guid.NewGuid();
            var movement = new RecordPlayerMovent(playerId: id
                                                , matchId: 1
                                                , latitude: 50.83386020243865
                                                , longitude: 4.440454779137023
                                                , recordedUtc:date
                                                );
            
            Assert.AreEqual(movement.RecordedUtc, date);

            var json = movement.AsJson();
            Assert.IsFalse(string.IsNullOrEmpty(json));
            var copy = json.IsValidJson(RecordPlayerMoventJsonContext.Default.RecordPlayerMovent, out var clone);
            Assert.IsNotNull(clone);
            Assert.IsTrue(copy);
            Assert.AreEqual(DateTimeKind.Utc, clone.RecordedUtc.Kind);
            Assert.AreEqual(movement.Latitude, clone.Latitude);
            Assert.AreEqual(movement.Longitude, clone.Longitude);
            Assert.AreEqual(movement.PlayerId, clone.PlayerId);
            Assert.AreEqual(movement.RecordedUtc, clone.RecordedUtc);
            Assert.AreNotEqual(DateTime.MinValue,clone.RecordedUtc);
            Assert.IsTrue(movement.Equals(clone));
        }


    }
}