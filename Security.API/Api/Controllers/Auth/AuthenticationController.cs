using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security.API.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Security.Api.Controllers.Auth
{
    [Authorize(AuthenticationSchemes = Framework.Consts.Constants.AUTH_BASIC_SCHEMA)]
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IUserService UserService;
        private readonly ITokenService TokenService;

        public AuthenticationController(IUserService userService, ITokenService tokenService)
        {
            this.UserService = userService;
            this.TokenService = tokenService;
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var usClaim = User.FindFirst(ClaimTypes.Name);
               
                var user = await UserService.GetUserByNameAsync(usClaim.Value);

                if (user is null)
                    return StatusCode(StatusCodes.Status401Unauthorized, new { message = "token creation failed.." });

                var tokenresult = await TokenService.GenerateTokenByUserAsyn(username: user.Username, user.Password);
                
                return Ok(tokenresult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }
}
