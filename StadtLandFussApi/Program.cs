﻿using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : new HerokuDbConnector.HerokuDbConnector().Build();

Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions = (_, builder) =>
    builder.UseNpgsql(connectionString, options => options.CommandTimeout(15))
    .UseSnakeCaseNamingConvention();

builder.Services.AddPooledDbContextFactory<AppDbContext>(dbContextOptions, poolSize: 20);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // do nothing now
}
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedProto
    });
    app.UseHttpsRedirection();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();
app.Run();
