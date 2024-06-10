namespace ClubStat.Infrastructure.Models.Tests
{
    using ClubStat.Infrastructure.Builder;
    using ClubStat.Infrastructure.Factories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
     
    [TestClass()]
    public class UpcomingMatchViewTest
    {
        [TestMethod()]
          public async Task ConstructorTest()
        {
           var config = new ConfigurationBuilder().AddJsonFile("Appsettings.json").Build();
            using var service = new ServiceCollection()
                            .RegisterClubStats(config).BuildServiceProvider();
            var user = await service.GetRequiredService<ILoginFactory>().Login("Ronaldo", "Ronaldo").ConfigureAwait(false);
            Assert.IsNotNull(user);
            var sut = service.GetRequiredService<IMatchFactory>();
            var match = await sut.GetPlyersNextMatch(user.UserId);
            Assert.IsNotNull(match);
            Assert.AreNotEqual(match.AwayTeam.ClubId, match.HomeTeam.ClubId);
          }
    }
}
