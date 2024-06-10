using ClubStat.Infrastructure.Builder;
using ClubStat.Infrastructure.Models;

using ClubStatUI.Builder;
using ClubStatUI.Pages;

using MetroLog.MicrosoftExtensions;
using MetroLog.Operators;

using Plugin.LocalNotification;

using System.Diagnostics;
namespace ClubStatUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            if (!CanSerialize())
            {
                throw new ApplicationException($"Can't serialize and communicate with the webserer");
            }
            string? dir = null;
            try
            {
//#if DEBUG
//                ClubStat.Infrastructure.Builder.ClubStatDependencyInjectionHelper.InjectFake = Debugger.IsAttached;
//#endif
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                        fonts.AddFont("Inter-Semibold.ttf", "InterSemibold");
                    })
                    .UseLocalNotification();


                builder.Services.RegisterClubStats(builder.Configuration);
                builder.Services.RegisterWindows();
                builder.Logging.AddStreamingFileLogger(
                options =>
                {
                    options.FolderPath = Path.Combine(
                    FileSystem.CacheDirectory,
                    "MetroLogs");
                    options.RetainDays = 2;
                    dir = options.FolderPath;
                    if (!Directory.Exists(options.FolderPath))
                    {
                        Directory.CreateDirectory(options.FolderPath);
                    }
                });
                //builder.Services.AddTransient<MainPage>();

                builder.Services.AddSingleton(LogOperatorRetriever.Instance);

                return builder.Build();
            }
            catch (Exception ex)
            {
                if (dir is not null)
                    File.WriteAllText(Path.Combine(dir, ex.GetType().Name + ".txt"), ex.ToString());
                throw;
            }
        }
        static bool CanSerialize()
        {
            var testUser = new LogInUser() { Password = "Ronaldo", Username = "Ronaldo" };
            var json = System.Text.Json.JsonSerializer.Serialize(testUser, LogInUserJsonContext.Default.LogInUser);
            var success = json.IsValidJson<LogInUser>(LogInUserJsonContext.Default.LogInUser, out var clone)
                && clone.Username.Equals(testUser.Username)
                && clone.Password.Equals(testUser.Password);

            return success;

        }
    }
}
