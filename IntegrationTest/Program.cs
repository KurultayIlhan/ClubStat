using ClubStat.Infrastructure.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using Walter;
namespace IntegrationTest
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                       .Enrich.FromLogContext()  
                       .WriteTo.File("AppData/Tests.txt"
                                    , buffered: true
                                    , flushToDiskInterval: TimeSpan.FromSeconds(30d)
                                    , rollingInterval: RollingInterval.Day
                                    , retainedFileCountLimit: 7
                                    , restrictedToMinimumLevel: LogEventLevel.Debug 
                                    , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                                    )   
                       .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            using var service = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddLogging(loggingBuilder =>{
                    
                    loggingBuilder.AddSerilog();
                })
                .RegisterClubStats(config)
                .BuildServiceProvider();

            service.AddLoggingForWalter();


            INSTRUCTIONS:
            Console.WriteLine("Testing the endpoints on:");
            Console.WriteLine("1. Player Location Recording REST API");
            Console.WriteLine("2. Get last location Via Rest Api");
            Console.WriteLine("3. Get record activity");
            Console.WriteLine("4. Run Record 10 Movements for Cîrlan Alexandru");
            Console.WriteLine("5. Get GetMotion statistics");

            while (true)
            {
                Console.WriteLine("Select any number to test or 0 to exit");
                var test = int.TryParse(Console.ReadLine(), out var choice);
                if (!test)
                { 
                    Console.Clear();
                    goto INSTRUCTIONS;
                }
                try
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Start Player Location Recording REST API");
                            await Tests.Recordings.ExecuteRecording(service);
                            break;
                        case 2:
                            Console.WriteLine("Get last location Via Rest Api");
                            await Tests.Recordings.ExecuteGetLast(service);
                            break;
                        case 3:
                            Console.WriteLine("Get record activity");
                            await Tests.Recordings.ExecuteGetRecordActivity(service);
                            break;
                            case 4:
                            Console.WriteLine("Run Record 10 Movements for Cîrlan Alexandru");
                            await Tests.Recordings.ExecuteRecordMovementSession(service);
                            break;
                            case 5:
                            await Tests.Recordings.ExecuteGetMotionstatistics(service);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }




        }
    }
}
