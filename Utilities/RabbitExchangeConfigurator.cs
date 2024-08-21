using DiscordButBetter.Server.Background;
using MassTransit;

namespace DiscordButBetter.Server.Utilities;

public static class RabbitExchangeConfigurator
{
    public static void ConfigureAllConsumers(this IBusRegistrationConfigurator cfg)
    {
        var type = typeof(IConsumer);
        var attributeType = typeof(UniqueEndpointAttribute);
        var types = 
            AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t) && Attribute.IsDefined(t, attributeType));

        foreach (var t in types)
        {
            cfg.AddConsumer(t).Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        }
    }
}