// ***********************************************************************
// Assembly         : 
// Author           : ILhan
// Created          : Thu 30-May-2024
//
// Last Modified By : WIlhan
// Last Modified On : Wed 12-Jun-2024
// ***********************************************************************
// <copyright file="MatchTests.cs" company="VIlhan">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Models.Tests
{

    [TestClass()]
    public class MatchTests
    {
        [TestMethod()]
        public void MatchTest()
        {
            var match = new Match(
                 new Club(clubId: 1, clubName: "HomeClub", clubIconUrl: "image/url.png", clubCity: "HomeCity")
                , new Club(clubId: 2, clubName: "AwayClub", clubIconUrl: "image/path.png", clubCity: "AwayCity")
                , playerLeague: 15
                , playerLeagueLevel: "A"
                , DateTime.Now.AddDays(-7)
                , awayTeamGoals: 5
                , homeTeamGoals: 4
                , matchId: 1);
            var json = match.AsJson();
            Assert.IsFalse(string.IsNullOrEmpty(json));
            var copy = json.IsValidJson<Match>(MatchJsonContext.Default.Match, out var clone);
            Assert.IsTrue(copy);
            Assert.IsNotNull(clone);

            Assert.IsTrue(match.Equals(clone));
            Assert.AreEqual(match.HomeTeamGoals, clone.HomeTeamGoals);
            Assert.AreEqual(match.PlayerLeague, clone.PlayerLeague);
            Assert.AreEqual(match.AwayTeamGoals, clone.AwayTeamGoals);
            Assert.AreEqual(match.HomeTeam.ClubIconUrl, clone.HomeTeam.ClubIconUrl);
            Assert.AreEqual(match.HomeTeam.ClubName, clone.HomeTeam.ClubName);
            Assert.AreEqual(match.HomeTeam.ClubCity, clone.HomeTeam.ClubCity);
            Assert.AreEqual(match.HomeTeam.ClubId, clone.HomeTeam.ClubId);

            Assert.AreEqual(match.AwayTeam.ClubIconUrl, clone.AwayTeam.ClubIconUrl);
            Assert.AreEqual(match.AwayTeam.ClubName, clone.AwayTeam.ClubName);
            Assert.AreEqual(match.AwayTeam.ClubCity, clone.AwayTeam.ClubCity);
            Assert.AreEqual(match.AwayTeam.ClubId, clone.AwayTeam.ClubId);
            Assert.AreEqual(match.MatchDate, clone.MatchDate);
            Assert.AreEqual(match.MatchId, clone.MatchId);
            Assert.AreEqual(match.GetReminderId(), clone.GetReminderId());

        }



    }
}