using Security.API.Application.DTOS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Security.API.Application.Interfaces
{
    public interface ITokenService
    {
        Task<UserDTO> GetUserByTokenAsync(string token);
        Task<bool> IsVallidTokenAsync(string token);
        Task<TokenGenerationResult> GenerateToken(TokenRequestDto request);

        Task<AuthenticationTicketResult> GenerateTokenByUserAsyn(string username, string password);
    }
}
