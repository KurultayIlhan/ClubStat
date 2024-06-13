using System.Reflection.Metadata;

using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClubStat.Infrastructure.Tests
{
    [TestClass]
    public class LoginRecordTest
    {
        [TestMethod]
        public async Task ConstructorTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("Appsettings.json").Build();
            using var service = new ServiceCollection()
                            .RegisterClubStats(config).BuildServiceProvider();


            var user = await service.GetRequiredService<ILoginFactory>().Login("Ronaldo", "Ronaldo").ConfigureAwait(false);

            Assert.IsNotNull(user);

            var login = new LoggedInUser(user);
            Assert.AreEqual(UserType.Player, login.UserType);
        }
    }

    [TestClass]
    public class TeamTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var sut = new Team() { Age=14, TeamAbrv = "B"};
            Assert.AreEqual("U15 B", sut.TeamName);
        }
    }
}