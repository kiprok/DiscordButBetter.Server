using Carter;
using DiscordButBetter.Server.Authentication;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAuthentication(AuthSchemeOptions.DefaultScheme)
    .AddScheme<AuthSchemeOptions, AuthHandler>(AuthSchemeOptions.DefaultScheme, options => { });
builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();


var db_host = Environment.GetEnvironmentVariable("DB_HOST") ?? builder.Configuration["DB_HOST"];
var db_port = Environment.GetEnvironmentVariable("DB_PORT") ?? builder.Configuration["DB_PORT"];
var db_name = Environment.GetEnvironmentVariable("DB_NAME") ?? builder.Configuration["DB_NAME"];
var db_user = Environment.GetEnvironmentVariable("DB_USER") ?? builder.Configuration["DB_USER"];
var db_pass = Environment.GetEnvironmentVariable("DB_PASS") ?? builder.Configuration["DB_PASS"];

var connectionString = $"Server={db_host};Port={db_port};Database={db_name};Uid={db_user};Pwd={db_pass};";
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 5));
builder.Services.AddDbContext<DbbContext>(options =>
{
   options.UseMySql( connectionString, serverVersion);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<NotificationHub>("/hub").RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapCarter();


app.Run();