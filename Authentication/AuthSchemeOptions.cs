using Microsoft.AspNetCore.Authentication;

namespace DiscordButBetter.Server.Authentication;

public class AuthSchemeOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "Basic";
    public const string AutherizationHeaderName = "Authorization";
}