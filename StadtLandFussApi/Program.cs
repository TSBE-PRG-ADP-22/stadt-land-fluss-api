﻿using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Hubs;
using StadtLandFussApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : new HerokuDbConnector.HerokuDbConnector().Build();

Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions = (_, builder) =>
    builder.UseNpgsql(connectionString, options => options.CommandTimeout(15))
    .UseSnakeCaseNamingConvention();

//builder.Services.AddPooledDbContextFactory<AppDbContext>(dbContextOptions, poolSize: 20);
builder.Services.AddDbContext<AppDbContext>(dbContextOptions, ServiceLifetime.Transient);
builder.Services.AddSignalR(options => options.EnableDetailedErrors = true).AddJsonProtocol();

builder.Services.AddCors(o => o.AddPolicy("ServicePolicy", builder =>
{
    builder
    .WithOrigins("http://localhost:3000", "https://stadt-land-fluss-ui.herokuapp.com")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
}));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dataContext.Database.Migrate();
}

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

app.UseCors("ServicePolicy");
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
});

app.UseAuthorization();
app.MapControllers();

app.MapHub<LobbyHub>("/lobby-hub");

app.Run();
