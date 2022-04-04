using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.API.Application.Interfaces;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Security.Api.Framework.Handlers
{
    public class OAuthBearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ITokenService TokenService;

        public OAuthBearerAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenService tokenService)
            : base(options, logger, encoder, clock)
        {

            TokenService = tokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var endpoint = Context.GetEndpoint();

                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                    return AuthenticateResult.NoResult();

                if (!Request.Headers.ContainsKey("Authorization"))
                    return AuthenticateResult.Fail("Authorization Header Invalid...");

                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                var isValid = await TokenService.IsVallidTokenAsync(authHeader.Parameter);

                if (!isValid)
                {
                    return AuthenticateResult.Fail("Invalid Token");
                }

                var user = await TokenService.GetUserByTokenAsync(authHeader.Parameter);

                if (user is null)
                    return AuthenticateResult.Fail("The token does not have a user");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, string.Join(",", user.Roles))
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);

                var principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    IssuedUtc = DateTime.Now,
                     
                }, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }
    }
}
