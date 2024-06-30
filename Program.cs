using System.Collections;
using Carter;
using DiscordButBetter.Server.Database;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

var db_host = Environment.GetEnvironmentVariable("DB_HOST") ?? builder.Configuration["DB_HOST"];
var db_port = Environment.GetEnvironmentVariable("DB_PORT") ?? builder.Configuration["DB_PORT"];
var db_name = Environment.GetEnvironmentVariable("DB_NAME") ?? builder.Configuration["DB_NAME"];
var db_user = Environment.GetEnvironmentVariable("DB_USER") ?? builder.Configuration["DB_USER"];
var db_pass = Environment.GetEnvironmentVariable("DB_PASS") ?? builder.Configuration["DB_PASS"];

Console.WriteLine($"DB_HOST: {db_host}");
Console.WriteLine($"DB_PORT: {db_port}");
Console.WriteLine($"DB_NAME: {db_name}");
Console.WriteLine($"DB_USER: {db_user}");
Console.WriteLine($"DB_PASS: {db_pass}");

var connectionString = $"Server={db_host};Port={db_port};Database={db_name};Uid={db_user};Pwd={db_pass};";
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