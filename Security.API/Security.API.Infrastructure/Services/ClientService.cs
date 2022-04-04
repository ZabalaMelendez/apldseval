using Security.API.Application.DTOS;
using Security.API.Application.Interfaces;
using Security.API.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Security.API.Application.Factories;
using Security.API.Application.Results;

namespace Security.API.Infrastructure.Services
{
    public class ClientService : IClientsService
    {
        private readonly SecurityApiDbContext _Db;

        public ClientService(SecurityApiDbContext db)
        {
            _Db = db;
        }

        /// <summary>
        /// List all client on DB
        /// NOT AWAITABLE
        /// </summary>
        /// <returns></returns>
        public List<ClientDTO> All()
        {
            return _Db.Clients.Select(d => new ClientDTO
            {
                AccessTokenExpiresInSeconds = d.AccessTokenExpiresInSeconds,
                Id = d.Id,
                Name = d.Name,
                RedirectUri = d.RedirectUri,
                RefreshTokenExpiresInSeconds = d.RefreshTokenExpiresInSeconds,
                Scopes = d.Scopes
            }).ToList();
        }

        /// <summary>
        /// create create for signup
        /// </summary>
        /// <param name="client">client to create</param>
        /// <returns></returns>
        public Task<ClientCreationResult> CreateClientAsync(ClientDTO client)
        {
            return Task.Run(() =>
            {

                if (_Db.Clients.Any(cl => cl.Name == client.Name))
                    throw new ApplicationException($"Client with name {client.Name} all ready exist..");

                var eDbClient = client.ToEntity();

                if (!eDbClient.IsValid())
                    throw new ApplicationException("Client is invalid..");

                eDbClient.ClientId = Guid.NewGuid().ToString();
                eDbClient.ClientSecred = Guid.NewGuid().ToString();

                _Db.Clients.Add(eDbClient);

                if (_Db.SaveChanges() > 0)
                    return new ClientCreationResult
                    {
                        ClientId = eDbClient.ClientId,
                        ClientSecret = eDbClient.ClientSecred
                    };
                else
                    throw new ApplicationException("client creation failed...");
            });
        }

        /// <summary>
        /// authenticate client on oauth server
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="clientsecret"></param>
        /// <returns></returns>
        public ClientDTO GetClient(string clientid, string clientsecret)
        {
            var client = _Db.Clients.FirstOrDefault(c => c.ClientId == clientid && c.ClientSecred == clientsecret);

            return client is null ? null : new ClientDTO
            {
                AccessTokenExpiresInSeconds = client.AccessTokenExpiresInSeconds,
                Name = client.Name,
                RefreshTokenExpiresInSeconds = client.RefreshTokenExpiresInSeconds,
                Scopes = client.Scopes,
                Id = client.Id
            };
        }

        /// <summary>
        /// Get client by it's client id
        /// </summary>
        /// <param name="clientid">client id to search</param>
        /// <returns>ClientDTO</returns>
        public async Task<ClientDTO> GetClientByClientIdAsync(string clientid)
        {
            var client = await Task.FromResult(_Db.Clients.FirstOrDefault(c => c.ClientId == clientid));

            if (client is null)
                return null;

            return new ClientDTO
            {
                AccessTokenExpiresInSeconds = client.AccessTokenExpiresInSeconds,
                Name = client.Name,
                RefreshTokenExpiresInSeconds = client.RefreshTokenExpiresInSeconds,
                Scopes = client.Scopes,
                Id = client.Id
            };
        }
    }
}
