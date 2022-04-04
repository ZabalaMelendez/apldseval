using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security.API.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Security.Api.Controllers.Auth
{
    [Route("[controller]")]
    [ApiController]
    public class OauthController : BaseController
    {
        private readonly ITokenService TokenService;


        public OauthController(ITokenService tokenService)
        {
            TokenService = tokenService;

        }

        [Authorize(AuthenticationSchemes = Framework.Consts.Constants.AUTH_BASIC_SCHEMA_OAUTH)]
        [Route("token")]
        [HttpPost]
        public async Task<IActionResult> Token([FromForm] string grant_type, [FromForm] string client_id,
           [FromForm] string username,
          [FromForm] string password,
          [FromForm] string refresh_token)
        {
            try
            {

                var result = await TokenService.GenerateToken(new API.Application.DTOS.TokenRequestDto
                {
                    ClientId = client_id,
                    GrantType = grant_type,
                    Password = password,
                    Username = username,
                    RefreshToken = refresh_token
                });

                return result.IsSuccess ? Ok(result.AuthenticationTicket)
                    : StatusCode(statusCode: result.StatusCode, result.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
