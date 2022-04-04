using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.API.Application.DTOS;
using Security.API.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Security.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService UserService;
        private readonly ILogger<UsersController> Logger;
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            UserService = userService;
            Logger = logger;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserDTO user)
        {
            Logger.LogInformation($"user creation start at {DateTime.Now:MMddyyyy hh:mm:ss tt}");

            try
            {
                System.Diagnostics.Debug.WriteLine(user.Username, user.Password, "Debug");

                var response = await UserService.CreateUserAsync(user);

                return Created("/Users/", response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ERROR:FATAL USER CREATION");
                return InternalServerError(ex);
            }
            finally
            {
                Logger.LogInformation($"user creation finish at {DateTime.Now:MMddyyyy hh:mm:ss tt}");
            }
        }

    }
}
