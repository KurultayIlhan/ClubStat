using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Walter;

namespace IntegrationTest.Tests
{
    internal class Recordings
    {
        public static async Task ExecuteRecording(IServiceProvider service)
        {
            var login = (await service!.GetRequiredService<ILoginFactory>()
                   .Login("Ronaldo", "Ronaldo")
                   .ConfigureAwait(false)) as Player;

            if (login is null) { Assert.Inconclusive("Login is not a player..."); }

            var match = await service.GetRequiredService<IMatchFactory>()
                                    .GetPlayersLastMatch(login.UserId)
                                     .ConfigureAwait(false);

            var sut = service.GetRequiredService<IPlayerRecorder>();
            Assert.IsNotNull(sut);
            var random = new Random();
            var lat = random.Next(50, 55) + random.NextDouble();
            var lng = random.Next(50, 55) + random.NextDouble();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Will generate a simulated location of {lat}/{lng}");
            Console.ForegroundColor = ConsoleColor.White;


            PlayerDynamicsLocation location = new(lat: Convert.ToDecimal(lat)
                                                , lng: Convert.ToDecimal(lng)
                                                , recorded: match!.MatchDate.AddSeconds(1)
                                                , matchId: match.MatchId
                                                , playerId: login.UserId);

            sut.RecordLocation(login, match, location);


            //wait for the background service to upload data
            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            Assert.AreEqual(sut.LastUpdate, location.Recorded);
            Assert.AreEqual(null, sut.LastRestError);
        }

        /// <summary>
        /// Executes the GetPlayersLastKnownLocation.
        /// </summary>
        /// <param name="service">The service.</param>
        public static async Task ExecuteGetLast(IServiceProvider service)
        {
            var login = (await service!.GetRequiredService<ILoginFactory>()
                   .Login("Ronaldo", "Ronaldo")
                   .ConfigureAwait(false)) as Player;

            if (login is null) { Assert.Inconclusive("Login is not a player..."); }

            var match = await service.GetRequiredService<IMatchFactory>()
                                    .GetPlayersLastMatch(login.UserId)
                                        .ConfigureAwait(false);

            Assert.IsNotNull(match, "No Match was returned");
            var sut = service.GetRequiredService<IPlayerRecorder>();

            Assert.IsNotNull(sut);
            var last = await sut.GetPlayersLastKnownLocation(player: login, match: match);
            Assert.IsNotNull(last, "Expect to have a last known location");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"on {last.RecordedUtc.ToLocalTime()} {login.FullName} was seen at {last.Location}");
            Console.ForegroundColor = ConsoleColor.White;


        }
        /// <summary>
        /// Executes the get record activity.
        /// </summary>
        /// <param name="service">The service.</param>
        public static async Task ExecuteGetRecordActivity(ServiceProvider service)
        {
            var login = (await service!.GetRequiredService<ILoginFactory>()
                  .Login("Ronaldo", "Ronaldo")
                  .ConfigureAwait(false)) as Player;

            if (login is null) { Assert.Inconclusive("Login is not a player..."); }

            var match = await service.GetRequiredService<IMatchFactory>()
                                    .GetPlayersLastMatch(login.UserId)
                                        .ConfigureAwait(false);
            Assert.IsNotNull(match, "No Match was returned");
            var sut = service.GetRequiredService<IPlayerRecorder>();
            var list = await sut.GetRecordActivity(login, match);
            Assert.IsNotNull(list, "The list has crashed");
            Assert.AreNotEqual(0, list.Count);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ik heb dit aantal: {list.Count}. List terug gekregen.");
            Console.ForegroundColor = ConsoleColor.White;

        }

        /// <summary>
        /// Executes the get motionstatistics test 5.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public static async Task ExecuteGetMotionstatistics(ServiceProvider service)
        {
            //we can test as many players as we like
            List<(string name, bool played, int matchid)> actors = new() {
                ("Haytam"  ,true    ,6),
                ("Messi"   ,false   ,6),
                ("Haytam"  ,false   ,1),
                ("Messi"   ,true    ,1),
                ("Fidel"   ,true    ,6),
            };

            var sut = service.GetRequiredService<IPlayerRecorder>();
            foreach (var (name, played, matchid) in actors)
            {
                await TestTheMatchMotion(service, sut, name, played, matchid).ConfigureAwait(false);

            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Tested {actors.Count} synchrounus");
            Console.ForegroundColor = ConsoleColor.White;
            await Parallel.ForEachAsync(actors, async (actor, cancellationToken) =>
            {
                await TestTheMatchMotion(service, sut, actor.name, actor.played, actor.matchid).ConfigureAwait(false);
            });


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Tested {actors.Count} in paralell");
            Console.ForegroundColor = ConsoleColor.White;


            //reuse test for paralell and simple synchrounus
            static async Task TestTheMatchMotion(ServiceProvider service, IPlayerRecorder sut, string name, bool played, int matchid)
            {
                var login = (await service!.GetRequiredService<ILoginFactory>()
                            .Login(name, name)
                            .ConfigureAwait(false)) as Player;

                if (login is null) { Assert.Inconclusive("Login is not a player..."); }
                var answer = await sut.GetPlayerMotionStatisticsAsync(login, matchId: matchid);
                Assert.IsNotNull(answer);
                Assert.AreEqual(2, answer.MatchId, $"Expecting to have match 2 data, have {answer.MatchId}");
                Assert.AreEqual(login.UserId, answer.PlayerId, $"Expecting to have player {login.FirstName} data, have received the wrong User ID {answer.PlayerId}");
                Assert.AreEqual(played, answer.PlayedInMatch, $"Player {name} is expected to {(played ? "play" : "not play")} in match 2");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{login.FullName} {(played ? "played" : " dit not play")} = Sprints: {answer.Sprints} MedianSpeed:{answer.MedianSpeed}  TopSpeed:{answer.TopSpeed}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        /// <summary>
        /// Executes the record movement session test nr 4.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public static async Task ExecuteRecordMovementSession(ServiceProvider service)
        {
            var locationJson = """[{"lat":50.7904037,"lng":4.2827311},{"lat":50.7905078,"lng":4.283048},{"lat":50.7905096,"lng":4.2830606},{"lat":50.7905128,"lng":4.283053},{"lat":50.7905129,"lng":4.28307},{"lat":50.7905134,"lng":4.2830588},{"lat":50.7905138,"lng":4.283058},{"lat":50.790514,"lng":4.2830674},{"lat":50.7905246,"lng":4.283059},{"lat":50.7905315,"lng":4.2830394}]""";

            var gameLocation = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Location>>(locationJson);
            Assert.IsNotNull(gameLocation);
            Assert.IsTrue(gameLocation.Count > 0);



            var playerRecorder = service.GetRequiredService<IPlayerRecorder>();
            Assert.IsNotNull(playerRecorder);

            var loginFactory = service.GetRequiredService<ILoginFactory>();
            var player = await loginFactory.Login("Cîrlan Alexandru", "Cîrlan Alexandru").ConfigureAwait(false) as Player;
            if (player is null) { Assert.Inconclusive("Login is not a player..."); }

            var matchFactory = service.GetRequiredService<IMatchFactory>();
            var match = await matchFactory.GetPlayersLastMatch(player.UserId).ConfigureAwait(false);
            Assert.IsNotNull(match);

            match.MatchDate = DateTime.Now.AddMinutes(-1);
            match.MatchId = 0;

            var gameTime = new Walter.TestClock(match.MatchDate);
            IClock clock = gameTime;

            foreach (var geo in gameLocation)
            {
                Assert.IsNotNull(geo);
                var fakeLocation = new PlayerDynamicsLocation
                {
                    Lat = geo.Lat,
                    Lng = geo.Lng,
                    MatchId = match.MatchId,
                    PlayerId = player.UserId,
                    Recorded = clock.Now
                };

                playerRecorder.RecordLocation(player, match, fakeLocation);
                gameTime.Add(TimeSpan.FromSeconds(1));
                await Task.Delay(1000).ConfigureAwait(false);
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            while (playerRecorder.HasData())
            {
                await Task.Delay(1000, cts.Token).ConfigureAwait(false);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Queue is processed for user {player.FullName}");
            Console.ResetColor();

            var lastKnownLocation = await playerRecorder.GetPlayersLastKnownLocation(player, match);
            Assert.IsNotNull(lastKnownLocation);

            var locationMatch = lastKnownLocation.MatchId == match.MatchId
                                && lastKnownLocation.PlayerId == player.UserId
                                && Math.Abs( lastKnownLocation.Latitude - gameLocation[^1].Lat)<1.001M
                                && Math.Abs( lastKnownLocation.Longitude - gameLocation[^1].Lng)<1.001M;

            Assert.IsTrue(locationMatch);
        }
    }



    public class Location
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

}
