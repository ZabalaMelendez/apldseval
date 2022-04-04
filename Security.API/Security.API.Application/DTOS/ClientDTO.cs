using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Application.DTOS
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Scopes { get; set; }
        public string RedirectUri { get; set; }
        public int AccessTokenExpiresInSeconds { get; set; }
        public int RefreshTokenExpiresInSeconds { get; set; }

    }
}
