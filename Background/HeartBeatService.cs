using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace DiscordButBetter.Server.Background;

public class HeartBeatService(
    ILogger<HeartBeatService> logger,
    IDbContextFactory<DbbContext> dbContextFactory,
    IServiceScopeFactory serviceScopeFactory)
    : IHostedService
{
    public static Guid ServiceId { get; } = Guid.NewGuid();
    public const int Interval = 5;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(Interval));
    private CancellationToken _cancellationToken;


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var server = new ServerModel
        {
            Id = ServiceId,
            LastPing = DateTime.UtcNow
        };

        await db.Servers.AddAsync(server, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        _ = Task.Run(DoWork, cancellationToken);
    }

    private async Task DoWork()
    {
        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(_cancellationToken);

            var servers = await db.Servers
                .Include(s => s.Connections)
                .Where(s => s.LastPing < DateTime.UtcNow.AddMinutes(-Interval * 2))
                .ToListAsync(_cancellationToken);
            await CleanUpServers(servers, db, _cancellationToken);

            while (await _timer.WaitForNextTickAsync(_cancellationToken))
            {
                var server = await db.Servers.FindAsync(ServiceId);
                server!.LastPing = DateTime.UtcNow;
                await db.SaveChangesAsync(_cancellationToken);

                servers = await db.Servers
                    .Include(s => s.Connections)
                    .Where(s => s.LastPing < DateTime.UtcNow.AddMinutes(-Interval * 2))
                    .ToListAsync(_cancellationToken);
                await CleanUpServers(servers, db, _cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Heartbeat service stopped");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        logger.LogInformation("Heartbeat service stopped");
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var server = await db.Servers
            .Include(s => s.Connections)
            .FirstOrDefaultAsync(s => s.Id == ServiceId, cancellationToken);
        if (server is null)
            return;

        db.Servers.Remove(server);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task CleanUpServers(List<ServerModel> servers, DbbContext db, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();

        var users = servers
            .SelectMany(s => s.Connections)
            .GroupBy(c => c.UserId)
            .Select(g => new { UserId = g.Key, count = g.Count() });

        var connections = db.Connections
            .Where(c => users.Select(u => u.UserId).Contains(c.UserId))
            .ToList();

        foreach (var user in users)
        {
            var connection = connections
                .Where(c => c.UserId == user.UserId)
                .ToList();
            if (connection.Count == user.count)
            {
                var dbUser = await db.Users.FindAsync(user.UserId);
                dbUser!.Online = false;
                dbUser.Status = 0;
                var userUpdate = new UserInfoChangedMessage()
                {
                    UserId = user.UserId,
                    Status = 0
                };
                await bus.Publish(userUpdate, cancellationToken);
            }
        }

        db.Servers.RemoveRange(servers);

        await db.SaveChangesAsync(cancellationToken);
    }
}