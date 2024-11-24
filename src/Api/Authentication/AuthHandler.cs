using System.Security.Claims;
using System.Text.Encodings.Web;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DiscordButBetter.Server.Authentication;

public class AuthHandler : AuthenticationHandler<AuthSchemeOptions>
{

    private readonly IUserService _userService;
    public AuthHandler(IOptionsMonitor<AuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUserService userService) : base(options, logger, encoder)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = "";
        
        if (Request.Query.ContainsKey("token"))
        {
            token = Request.Query["token"];
        }else if (Request.Headers.ContainsKey(AuthSchemeOptions.AuthorizationHeaderName))
        {
            token = Request.Headers[AuthSchemeOptions.AuthorizationHeaderName];
        }
        else
        {
            return AuthenticateResult.Fail("Unauthorized");
        }


        
        var session = _userService.Authenticate(token.ToString());
        if (session == null)
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        var claims = new List<Claim>
        {
            new("UserId", session.userId.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }
}