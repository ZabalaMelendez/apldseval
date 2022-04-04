using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Security.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        protected virtual IActionResult InternalServerError(Exception ex)
            => StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, ex);
    }
}
