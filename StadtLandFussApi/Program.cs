using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Hubs;
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

//builder.Services.AddPooledDbContextFactory<AppDbContext>(dbContextOptions, poolSize: 20);
builder.Services.AddDbContext<AppDbContext>(dbContextOptions);
builder.Services.AddSignalR();


builder.Services.AddCors(o => o.AddPolicy("ServicePolicy", builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
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
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();

app.MapHub<LobbyHub>("/user-added");

app.Run();
