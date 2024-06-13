// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Fri 31-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="ProfilePictureFactoryTests.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.RestServer;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace ClubStat.Infrastructure.Factories.Tests;

[TestClass()]
public class ProfilePictureFactoryTests : IntegrationTestFixture
{
    private HttpClient? _client;

    [TestInitialize]
    public void TestInitialize()
    {
        _client = base.CreateClient();
    }

    [TestMethod()]
    public async Task GetProfilePictureForUserAsyncTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        using var service = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .RegisterClubStats(config)
            .AddSingleton<IHttpClientFactory>(this)
            .BuildServiceProvider();

        var sut = service.GetService<IProfilePictureFactory>();
        Assert.IsNotNull(sut);
        var player = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6");
        var image = await sut.GetProfilePictureForUserAsync(player);
        Assert.IsNotNull(image);
        Assert.AreNotEqual(0L, image.LongLength);


    }
    [TestMethod]
    public async Task UploadPlayerTest()
    {
        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "Players");
        var imgDir = new DirectoryInfo(imgPath);
        var files = imgDir.GetFiles("*.png");
        var images = new Queue<string>(files.Select(file => file.FullName));
        if (images.Count == 0) Assert.Inconclusive("Can't find images");


        var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Live.json")
        .Build();

        using var service = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .RegisterClubStats(config)
            .AddSingleton<IHttpClientFactory>(this)
            .BuildServiceProvider();

        var club = service.GetRequiredService<IClubFactory>();
        var db = service.GetRequiredService<IProfilePictureFactory>();
        var team = await club.GetTeamPlayers(3, 14, 'B').ConfigureAwait(false);
        if(team.Count == 0)
        {
            Assert.Inconclusive("Can call team");
        }
        foreach (var player in team)
        {
            if (player == null) continue;
            if (images.TryDequeue(out var path))
            {
                var pictureData = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
                if (pictureData != null)
                {
                    await db.UploadPictureForUserAsync(player.UserId, pictureData).ConfigureAwait(false);

                    var copy = await db.GetProfilePictureForUserAsync(player.UserId).ConfigureAwait(false);
                    Assert.IsTrue(pictureData.SequenceEqual(copy));
                }
            }
            else
            {
                break;
            }
        }
        team = await club.GetTeamPlayers(4, 15, 'A').ConfigureAwait(false);
        foreach (var player in team)
        {
            if (player == null) continue;
            if (images.TryDequeue(out var path))
            {
                var pictureData = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
                if (pictureData != null)
                {
                    await db.UploadPictureForUserAsync(player.UserId, pictureData).ConfigureAwait(false);

                    var copy = await db.GetProfilePictureForUserAsync(player.UserId).ConfigureAwait(false);
                    Assert.IsTrue(pictureData.SequenceEqual(copy));
                }
            }
            else
            {
                break;
            }
        }
    }

}

public class IntegrationTestFixture : WebApplicationFactory<ClubStat.RestServer.Program>, IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return base.CreateClient();
    }
    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => { Program.SetupServices(services); });
        return base.CreateServer(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
         {
             services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

         });

        builder.Configure(app =>
        {
            app.Use(next => context =>
            {
                context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
                return next(context);
            });
            Program.SetupMiddleware(app);
        });

    }

}