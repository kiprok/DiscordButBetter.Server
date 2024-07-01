using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DiscordButBetter.Server.Authentication;

public class AuthHandler : AuthenticationHandler<AuthSchemeOptions>
{

    public AuthHandler(IOptionsMonitor<AuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }
}