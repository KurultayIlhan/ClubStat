using ClubStat.Infrastructure.Builder;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using ClubStatUI.Builder;

using Microsoft.Extensions.Configuration;

namespace ClubStatUI.ViewModels.Tests
{
    [TestClass()]
    public class DashboardPlayerViewModelTests
    {
        [TestMethod()]
        public async Task DashboardPlayerViewModelTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("Appsettings.json").Build();
            using var service = new ServiceCollection()
                            .RegisterClubStats(config)
                            .RegisterWindows()
                            .BuildServiceProvider();


            var user = await service.GetRequiredService<ILoginFactory>().Login("Ronaldo", "Ronaldo").ConfigureAwait(false);
            DashboardPlayerViewModel sut = service.GetRequiredService<DashboardPlayerViewModel>();
            Assert.IsNotNull(sut.User);
            Assert.AreNotEqual(string.Empty, user?.FullName);
            
            await sut.ExecuteAsync();
            Assert.IsTrue(sut.UpcomingMatch?.Match?.MatchDate > DateTime.Now);
            Assert.IsNotNull(sut.UpcomingMatch?.Match?.HomeTeam);
            Assert.IsNotNull(sut.UpcomingMatch?.Match?.AwayTeam);
            Assert.IsFalse( string.IsNullOrEmpty( sut.UpcomingMatch?.Match?.AwayTeam.ClubName));
            Assert.IsFalse( string.IsNullOrEmpty( sut.UpcomingMatch?.Match?.AwayTeam.ClubIconUrl));
            Assert.AreEqual(0,  sut.UpcomingMatch?.Match?.AwayTeam.ClubId);
            Assert.IsFalse( string.IsNullOrEmpty(  sut.UpcomingMatch?.Match?.AwayTeam.ClubCity));

            Assert.IsFalse( string.IsNullOrEmpty( sut.UpcomingMatch?.Match?.HomeTeam.ClubName));
            Assert.IsFalse( string.IsNullOrEmpty( sut.UpcomingMatch?.Match?.HomeTeam.ClubIconUrl));
            Assert.AreEqual(0,  sut.UpcomingMatch?.Match?.HomeTeam.ClubId);
            Assert.IsFalse( string.IsNullOrEmpty(  sut.UpcomingMatch?.Match?.HomeTeam.ClubCity));


            Assert.AreNotEqual(0,  sut.UpcomingMatch?.Match?.MatchId);
            Assert.AreEqual(0,  sut.UpcomingMatch?.Match?.HomeTeamGoals);
            Assert.AreEqual(0,  sut.UpcomingMatch?.Match?.AwayTeamGoals);
            Assert.AreNotEqual<Division>(Division.None, sut.UpcomingMatch!.Division);


        }


    }
}