using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Api.Models;

namespace Security.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistsController : ControllerBase
    {
        private static readonly List<ArtistsResponse> artists;

        static ArtistsController()
        {
            artists = new List<string>()
            {
                "Graham", "Jessica", "Paul", "Kenneth", "Werner", "Mary", "Frances", "Yvonne", "Catherine", "Tonya"
            }
            .Select(name => new ArtistsResponse { Name = name })
            .ToList();
        }


        private readonly ILogger<ArtistsController> _logger;

        public ArtistsController(ILogger<ArtistsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<ArtistsResponse> Get()
        {
            return artists;
        }

        [Authorize(AuthenticationSchemes = Security.Api.Framework.Consts.Constants.SCHEMA_OAUTH)]
        [HttpGet]
        [Route("{name}")]
        public ActionResult<ArtistsResponse> Get(string name)
        {
            var response = artists.Find(currentArtists => currentArtists.Name.Equals(name));

            if (response == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(response);
        }

        [Authorize(Policy = "RequiredAdminRolePO", AuthenticationSchemes = Security.Api.Framework.Consts.Constants.SCHEMA_OAUTH)]
        [HttpPost]
        public ActionResult<ArtistsResponse> Post([FromBody] CreateArtistRequest artist)
        {
            var newArtists = new ArtistsResponse
            {
                Name = artist.Name
            };

            artists.Add(newArtists);

            return Created("/", newArtists);
        }
    }
}
