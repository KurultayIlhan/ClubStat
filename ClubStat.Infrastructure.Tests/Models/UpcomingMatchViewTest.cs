namespace ClubStat.Infrastructure.Models.Tests
{
    [TestClass()]
    public class UpcomingMatchViewTest
    {
        [TestMethod()]
        public void ConstructorTest()
        {
            var sut = new UpcomingMatchView();
            Assert.IsNotNull(sut);
            Assert.AreEqual(Division.None,sut.Division);
        }
    }
}