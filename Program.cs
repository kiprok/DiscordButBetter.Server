using System.Text.Json.Serialization;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Carter;
using DiscordButBetter.Server.Authentication;
using DiscordButBetter.Server.Background;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Services;
using MassTransit;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddLogging();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAuthentication(AuthSchemeOptions.DefaultScheme)
    .AddScheme<AuthSchemeOptions, AuthHandler>(AuthSchemeOptions.DefaultScheme, options => { });
builder.Services.AddAuthorization();
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
});

var credentials = new BasicAWSCredentials(
    builder.Configuration["AWS_ACCESS_KEY"],
    builder.Configuration["AWS_SECRET_KEY"]);

builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Credentials = credentials,
    DefaultClientConfig =
    {
        ServiceURL = "https://1b7039981caabc1fc0f00dabaa35bc42.r2.cloudflarestorage.com"
    }
});

AWSConfigsS3.UseSignatureVersion4 = true;

var connectionString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
    builder.Configuration["DB_HOST"],
    builder.Configuration["DB_PORT"],
    builder.Configuration["DB_NAME"],
    builder.Configuration["DB_USER"],
    builder.Configuration["DB_PASS"]);

var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContextFactory<DbbContext>(options => { options.UseMySql(connectionString, serverVersion); });
builder.Services.AddHostedService<HeartBeatService>();

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
app.UseSpa(option => { option.Options.SourcePath = "wwwroot"; });

app.MapCarter();


app.Run();