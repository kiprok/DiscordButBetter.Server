namespace DiscordButBetter.Server.Utilities;

public static class AppSettings
{
    public static string ServiceId { get; private set; }

    public static void Initialize(string serviceId)
    {
        ServiceId = serviceId;
    }
}