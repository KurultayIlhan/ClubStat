// ***********************************************************************
// Assembly         : ClubStat.RestServer
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="Program.cs" company="ClubStat.RestServer">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary>
//  Add support for integration tests by wrapping it in a class and making it compatible with 
// Microsoft.AspNetCore.Mvc.Testing and WebApplicationFactory
// </summary>
// ***********************************************************************

using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Builder;

using MailKit.Security;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using Serilog;
using Serilog.Enrichers.WithCaller;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;

using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ClubStat.RestServer;

public class Program
{


    public static void Main(string[] args)
    {
        var emailConnectionInfo = new EmailSinkOptions
        {
            From = "clubstat72@gmail.com",
            To = new List<string>() { "clubstat72@gmail.com", "wave@vesnx.com" },
            Host = "smtp.gmail.com",
            Credentials = new System.Net.NetworkCredential
            {
                UserName = "clubstat72@gmail.com",
                Password = "ClubStat123"
            },

            Port = 465,
            IsBodyHtml = false,
            ConnectionSecurity = SecureSocketOptions.Auto,

            Subject = new MessageTemplateTextFormatter("ClubStat-RestServer Serilog Email Log")
        };
        emailConnectionInfo.ServerCertificateValidationCallback += OnCallback;

#if DEBUG
        var level = LogEventLevel.Debug;
#else
        var level = LogEventLevel.Information;
#endif

        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .Enrich.FromLogContext()
           .Enrich.WithCaller(true)
           .Enrich.WithExceptionDetails()
           .WriteTo.Console()

           .WriteTo.File("AppData/log.txt"
                        , buffered: true
                        , flushToDiskInterval: TimeSpan.FromSeconds(30d)
                        , rollingInterval: RollingInterval.Day
                        , retainedFileCountLimit: 7
                        , restrictedToMinimumLevel: level
                        , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                        )
           .WriteTo.Email(emailConnectionInfo, restrictedToMinimumLevel: LogEventLevel.Error) // Send only Error level and above logs via email
           .CreateLogger();


        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSerilog(Log.Logger);
        builder.Host.UseSerilog(Log.Logger);

        var sw = Stopwatch.StartNew();
        try
        {
            Log.Logger.Information("Configuring Dependency injection ClubStats Rest Server");
            SetupServices(builder.Services);

            var app = builder.Build();
            Log.Logger.Information("Configuring Middleware on the ClubStats Rest Server:{Elapsed}", sw.Elapsed);
            SetupMiddleware(app);



            Log.Logger.Information("Starting ClubStats Rest Server:{Elapsed}", sw.Elapsed);
            app.Run();

            sw.Stop();
            Log.Logger.Information("Clean exit of the ClubStats Rest Server after {Elapsed}", sw.Elapsed);

        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "An fatal {error} occurred while starting or during the execution of the ClubStats Rest Server:{Message}"
                , ex.GetType().Name, ex.Message);
        }
        finally
        {
            if (sw.IsRunning)
            {
                Log.Logger.Fatal($"Flush and Close while stop was not triggered:{sw.Elapsed}");
            }
            else
            {
                Log.Logger.Information($"Flush and Close :{sw.Elapsed}");
            }
            Log.CloseAndFlush();
        }
    }

    private static bool OnCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            Log.Logger.Error("{sslPolicyErrors} error on connecting to mail server", sslPolicyErrors);
        }

        return true;
    }

    public static void SetupMiddleware(IApplicationBuilder app)
    {

        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();
        //manage rest api key as well Authorization session
        app.UseMiddleware<ClubStat.RestServer.Infrastructure.SecurityMiddleware>();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

    }

    public static void SetupServices(IServiceCollection services)
    {

        services.AddControllers(options =>
        {
            options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddWebsiteInfrastructure();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Add(CoachJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(GetTeamPlayersRequestContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(LogInResultJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(LogInUserJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(MatchAgendaJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerActivityRowJsonContext.Default);//single
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerActivityRowsJsonContext.Default);//list
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerDynamicsLocationJsonContext.Default);//single
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerDynamicsLocationsJsonContext.Default);//list
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerGameMotivationJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerMatchPositionJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerMatchPositionListJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerMotionStatisticsJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerMovementJsonContext.Default);//single PlayerMovement
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerMovementsJsonContext.Default);//list of PlayerMovement
            options.SerializerOptions.TypeInfoResolverChain.Add(PlayerStatisticsJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(ProfileImageJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(RecordPlayerMoventJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(TeamOfPlayerJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Add(UpdatePasswordJsonContext.Default);
        });

    }

}


