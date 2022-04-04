using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.API.Application.DTOS;
using Security.API.Application.Interfaces;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Security.Api.Framework.Handlers
{
    public class OAuthBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IClientsService clientsService;

        public OAuthBasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions>
            options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IClientsService clientsService) : base(options, logger, encoder, clock)
        {
            this.clientsService = clientsService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Authorization Header Invalid...");

            ClientDTO client = null;

            var clientSecret = string.Empty;

            var clientId = string.Empty;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);

                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);

                clientId = credentials[0];

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    await Response.WriteAsync("Invalid ClientId");
                    Response.ContentType = "application/json";
                    return AuthenticateResult.Fail("Invalid password");
                }

                clientSecret = credentials[1];

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    await Response.WriteAsync("Invalid password");
                    Response.ContentType = "application/json";
                    return AuthenticateResult.Fail("Invalid password");
                }

                client = await Task.FromResult(clientsService.GetClient(clientId, clientSecret));
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if(client is null && !Guid.TryParse(clientId, out Guid resultguid))
            {
                var resultINvliad = AuthenticateResult.Fail("Invalid Username");
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("Invalid Username");
                Response.ContentType = "application/json";
                return resultINvliad;
            }


            if (client is null && !Guid.TryParse(clientSecret, out Guid passguid))
            {
                var resultINvliad = AuthenticateResult.Fail("Invalid Password");
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("Invalid Password");
                Response.ContentType = "application/json";
                return resultINvliad;
            }

            if (client == null)
            {
                var result = AuthenticateResult.Fail("Invalid Username or Password");
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("Invalid Username or Password");
                Response.ContentType = "application/json";
                return result;
            }

            var claims = new[] {

                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                new Claim(ClaimTypes.Name, client.Name),
                new Claim(ClaimTypes.Role, "CLIENT"),
                new Claim(ClaimTypes.Surname, clientId)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }
    }
}
