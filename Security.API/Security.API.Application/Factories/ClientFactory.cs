using Security.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Security.API.Application.DTOS;

namespace Security.API.Application.Factories
{
    public static class ClientFactory
    {

        public static Client ToEntity(this ClientDTO client)
            => new Client
            {
                //ClientId = client.ClientId,
                AccessTokenExpiresInSeconds = client.AccessTokenExpiresInSeconds,
                //ClientSecred = client.ClientSecred,
                Name = client.Name,
                RedirectUri = client.RedirectUri,
                RefreshTokenExpiresInSeconds = client.RefreshTokenExpiresInSeconds,
                Scopes = client.Scopes
            };


        public static Client ToEntity(this ClientDTO client, Guid clientid, Guid clientsecret)
            => new Client
            {
                ClientId = clientid.ToString(),
                AccessTokenExpiresInSeconds = client.AccessTokenExpiresInSeconds,
                ClientSecred = clientsecret.ToString(),
                Name = client.Name,
                RedirectUri = client.RedirectUri,
                RefreshTokenExpiresInSeconds = client.RefreshTokenExpiresInSeconds,
                Scopes = client.Scopes
            };

    }
}
