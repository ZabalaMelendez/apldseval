using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.API.Application.Interfaces;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using Security.API.Domain.Entities;
using Security.API.Application.DTOS;

namespace Security.Api.Framework.Handlers
{

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService UserService;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserService userService) : base(options, logger, encoder, clock)
        {
            this.UserService = userService;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Authorization Header Invalid...");

            UserDTO user = null;
            var password = string.Empty;
            var username = string.Empty;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                username = credentials[0];

                if (string.IsNullOrWhiteSpace(username))
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    await Response.WriteAsync("Invalid Username");
                    Response.ContentType = "application/json";
                    return AuthenticateResult.Fail("Invalid password");
                }

                password = credentials[1];

                if (string.IsNullOrWhiteSpace(password))
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    await Response.WriteAsync("Invalid password");
                    Response.ContentType = "application/json";
                    return AuthenticateResult.Fail("Invalid password");
                }
                user = await UserService.Authenticate(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null)
            {
                var result = AuthenticateResult.Fail("Invalid Username or Password");
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("Invalid Username or Password");
                Response.ContentType = "application/json";
                return null;
            }

            var claims = new[] {

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role,  string.Join(",",user.Roles))
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }
    }
}
