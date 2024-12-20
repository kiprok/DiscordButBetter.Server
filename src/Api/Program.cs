using System.Text.Json.Serialization;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Carter;
using DiscordButBetter.Server;
using DiscordButBetter.Server.Authentication;
using DiscordButBetter.Server.Background;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Services;
using DiscordButBetter.Server.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

AppSettings.Initialize(builder.Configuration["SERVICE_ID"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddLogging();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

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
    x.ConfigureAllConsumers();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RABBITMQ_HOST"], "/", h =>
        {
            h.Username(builder.Configuration["RABBITMQ_USER"]);
            h.Password(builder.Configuration["RABBITMQ_PASS"]);
        });


        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
    });
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

var connectionString = string.Format("Host={0};Port={1};Database={2};Username={3};Password={4};",
    builder.Configuration["DB_HOST"],
    builder.Configuration["DB_PORT"],
    builder.Configuration["DB_NAME"],
    builder.Configuration["DB_USER"],
    builder.Configuration["DB_PASS"]);
Console.WriteLine(connectionString);

//var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContextFactory<DbbContext>(options => { options.UseNpgsql(connectionString); });
builder.Services.AddHostedService<HeartBeatWorker>();

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