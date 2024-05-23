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
// <summary></summary>
// ***********************************************************************

using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWebsiteInfrastructure();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(LogInUserJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(LogInResultJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(PlayerJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(UpdatePasswordJsonContext.Default);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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

app.MapControllers();

app.Run();
