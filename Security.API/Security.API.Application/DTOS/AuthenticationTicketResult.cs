using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Application.DTOS
{
    public class TokenGenerationResult
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public virtual AuthenticationTicketResult AuthenticationTicket { get; set; }

        public static TokenGenerationResult Fail(int code, string message)
            => new TokenGenerationResult
            {
                AuthenticationTicket = new AuthenticationTicketResult { },
                IsSuccess = false,
                Message = message,
                StatusCode = code
            };
    }

    public class AuthenticationTicketResult
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public string Refresh_token { get; set; }
        public int Expires_in { get; set; }
        public string Scope { get; set; }
    }
}
