using System.Reflection.Metadata;
using ClubStat.Infrastructure.Models;

namespace ClubStat.Infrastructure.Tests
{
    [TestClass]
    public class LoginRecordTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var login = new LoggedInUser("username",UserType.Player);
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