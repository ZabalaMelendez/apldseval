using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ClientsController : BaseController
    {
        private readonly IClientsService ClientsService;

        public ClientsController(IClientsService clientsService)
        {
            ClientsService = clientsService;
        }


        [HttpGet]
        public ActionResult<List<ClientDTO>> Get()
            => ClientsService.All();


        [HttpPost]
        public async Task<IActionResult> Post(ClientDTO request)
        {
            try
            {
                var response = await ClientsService.CreateClientAsync(request);

                return Created("/Clients/",response);

            }
            catch (ApplicationException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
