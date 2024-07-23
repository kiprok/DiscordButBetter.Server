using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
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

var awsAccessKey = builder.Configuration["AWS_ACCESS_KEY"];
var awsSecretKey = builder.Configuration["AWS_SECRET_KEY"];
Console.WriteLine(awsAccessKey!.Length);
Console.WriteLine(awsSecretKey!.Length);
var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Credentials = credentials,

    DefaultClientConfig =
    {
        ServiceURL = "https://1b7039981caabc1fc0f00dabaa35bc42.r2.cloudflarestorage.com"
    }
});
AWSConfigsS3.UseSignatureVersion4 = true;

var db_host = builder.Configuration["DB_HOST"];
var db_port = builder.Configuration["DB_PORT"];
var db_name = builder.Configuration["DB_NAME"];
var db_user = builder.Configuration["DB_USER"];
var db_pass = builder.Configuration["DB_PASS"];

var connectionString = $"Server={db_host};Port={db_port};Database={db_name};Uid={db_user};Pwd={db_pass};";
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 5));
builder.Services.AddDbContext<DbbContext>(options => { options.UseMySql(connectionString, serverVersion); });

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