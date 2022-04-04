using Security.API.Application.DTOS;
using Security.API.Application.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Security.API.Application.Interfaces
{
    public interface IClientsService
    {
        ClientDTO GetClient(string clientid, string clientsecret);
        List<ClientDTO> All();
        Task<ClientCreationResult> CreateClientAsync(ClientDTO client);
        Task<ClientDTO> GetClientByClientIdAsync(string clientid);
    }
}
