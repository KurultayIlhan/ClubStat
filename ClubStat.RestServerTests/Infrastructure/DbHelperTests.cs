using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClubStat.RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClubStat.RestServer.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClubStat.Infrastructure.Models;
using System.Reflection;

namespace ClubStat.RestServer.Infrastructure.Tests
{
    [TestClass()]
    public class DbHelperTests
    {
        [TestMethod()]
        public async Task LoginCoachAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var succesFullLogin = new LogInUser() { Username = "Yohan", Password = "Yohan" };
            var result = await db.LoginUserAsync(succesFullLogin);
            Assert.IsTrue(result.UserType == UserType.Coach);
        }
        [TestMethod()]
        public async Task LoginPlayerAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var succesFullLogin = new LogInUser() { Username = "Ronaldo", Password = "Ronaldo" };
            var result = await db.LoginUserAsync(succesFullLogin);
            Assert.IsTrue(result.UserType == UserType.Player);
        }
        [TestMethod()]
        public async Task FailedLoginUserAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var succesFullLogin = new LogInUser() { Username = "Wrong User", Password = "WrongPassword" };
            var result = await db.LoginUserAsync(succesFullLogin);
            Assert.IsTrue(result.UserType == UserType.None);
        }

        [TestMethod()]
        public async Task GetPlayerAsyncTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            using var service = new ServiceCollection()
                .AddWebsiteInfrastructure()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();
            var db = service.GetService<DbHelper>();
            Assert.IsNotNull(db);
            var expect = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6");
            var ronaldo = await db.GetPlayerAsync(expect);
            Assert.IsNotNull(ronaldo);
            Assert.AreEqual(expect, ronaldo.UserId);
        }
    }
}