using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClubStat.Infrastructure.Factories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClubStat.Infrastructure.Models;

namespace ClubStat.Infrastructure.Factories.Tests
{
    [TestClass()]
    public class PlayerRecorderTests
    {
        static TestContext? _context;
        static ServiceProvider? _service;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _context = context;
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.Live.json")
                            .Build();

            _service = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .RegisterClubStats(config)                
                .BuildServiceProvider();
            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _service?.Dispose();
        }

        [TestMethod()]
        public async Task PlayerRecorderTest()
        {
            if(_service is null)  Assert.Inconclusive("DI is not available...");

            var login = (await _service!.GetRequiredService<ILoginFactory>()
                               .Login("Ronaldo","Ronaldo")
                               .ConfigureAwait(false)) as Player;

            if (login is null) { Assert.Inconclusive("Login is not a player..."); }
            var match= await _service.GetRequiredService<IMatchFactory>()
                                     .GetPlayersLastMatch(login.UserId)
                                     .ConfigureAwait(false);

            var sut = _service.GetRequiredService<IPlayerRecorder>();
            Assert.IsNotNull(sut);
            PlayerDynamicsLocation location = new(5.0M, 4.2M, match!.MatchDate.AddSeconds(1), login.UserId); 
            sut.RecordLocation(login, match, location);

            //wait for the background service to upload data
            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.AreEqual(sut.LastUpdate,location.Recorded);


        }

        
    }
}