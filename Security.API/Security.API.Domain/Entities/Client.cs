using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Domain.Entities
{
    public class Client : Entity
    {
        public string Name { get; set; }
        public string Scopes { get; set; }
        public string RedirectUri { get; set; }
        public int AccessTokenExpiresInSeconds { get; set; }
        public int RefreshTokenExpiresInSeconds { get; set; }
        public string ClientId { get; set; }
        public string ClientSecred { get; set; }


        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(RedirectUri);
        }
    }
}
