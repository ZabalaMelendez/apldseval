using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Application.DTOS
{
    public class TokenRequestDto
    {
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string RefreshToken { get; set; }

    }
}
