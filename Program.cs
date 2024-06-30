using Carter;
using DiscordButBetter.Server.Database;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

var connectionString = $"server={Environment.GetEnvironmentVariable("DB_HOST", EnvironmentVariableTarget.User)};port={Environment.GetEnvironmentVariable("DB_PORT", EnvironmentVariableTarget.User)};database={Environment.GetEnvironmentVariable("DB_NAME", EnvironmentVariableTarget.User)};user={Environment.GetEnvironmentVariable("DB_USER", EnvironmentVariableTarget.User)};password={Environment.GetEnvironmentVariable("DB_PASS", EnvironmentVariableTarget.User)}";
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 5));
builder.Services.AddDbContext<DbbContext>(options =>
{
    // Gets connection Details from environment variables
   options.UseMySql( connectionString, serverVersion);
});

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.Run();