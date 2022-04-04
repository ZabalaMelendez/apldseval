using Microsoft.EntityFrameworkCore;
using Security.API.Application.DTOS;
using Security.API.Application.Framework;
using Security.API.Application.Interfaces;
using Security.API.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.API.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUserService Userservice;
        private readonly SecurityApiDbContext Db;
        private readonly IClientsService ClientsService;

        public TokenService(IUserService userService,
            SecurityApiDbContext securityApiDbContext,
            IClientsService clientsService)
        {
            Userservice = userService;
            Db = securityApiDbContext;
            ClientsService = clientsService;
        }

        public async Task<bool> IsVallidTokenAsync(string token)
        {
            var tokenDb = await Task.FromResult(Db.Tokens.FirstOrDefault(t => t.AccessToken == token));

            bool isValidToken = tokenDb != null
                && Guid.TryParse(token, out Guid rf)
                && !(token == Guid.Empty.ToString());

            int seconds = Convert.ToInt32(DateTime.Now.Subtract(tokenDb.GenerateAt).TotalSeconds);

            return isValidToken && tokenDb.ExpireAt > seconds;

        }

        public async Task<UserDTO> GetUserByTokenAsync(string token)
        {
            var tokenFromDb = await Db.Tokens.FirstOrDefaultAsync(t => t.AccessToken == token);

            if (tokenFromDb != null)
            {
                var user = await Userservice.GetUserByIdAsync(tokenFromDb.UserId);
                return user;
            }
            else
                return null;
        }



        /// <summary>
        /// generate new and refresh tokens
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TokenGenerationResult> GenerateToken(TokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.GrantType))
                return TokenGenerationResult.Fail(400, "invalid grand_type parameter required");

            if (request.GrantType == GRANT_TYPES.PASSWORD)
            {

                if (string.IsNullOrWhiteSpace(request.Username))
                    return TokenGenerationResult.Fail(400, "user does not exist"); //BADREQUEST

                var user = await Userservice.GetUserByNameAsync(request.Username);

                if (user is null)
                    return TokenGenerationResult.Fail(400, "User does not exist...");

                var client = await ClientsService.GetClientByClientIdAsync(request.ClientId);

                if (string.IsNullOrWhiteSpace(request.ClientId))
                    return TokenGenerationResult.Fail(401, "client does not exit");

                if (client is null)
                    return TokenGenerationResult.Fail(401, "client does not exit");

                var authUser = await Userservice.Authenticate(request.Username, request.Password);

                if (authUser is null)
                    return TokenGenerationResult.Fail(400, "wrong user credentials");

                var token = new AuthenticationTicketResult
                {
                    Access_token = Guid.NewGuid().ToString(),
                    Expires_in = Convert.ToInt32(TimeSpan.FromSeconds(client.AccessTokenExpiresInSeconds).TotalSeconds),
                    Refresh_token = Guid.NewGuid().ToString(),
                    Scope = Constants.DEFAULT_SCOPE,
                    Token_type = Constants.TOKEN_TYPE_BR
                };

                await Db.Tokens.AddAsync(new Domain.Entities.Token
                {
                    AccessToken = token.Access_token,
                    CreateAt = DateTime.Now,
                    ExpireAt = token.Expires_in,
                    GenerateAt = DateTime.Now,
                    Grant = GRANT_TYPES.PASSWORD,
                    RefreshToken = token.Refresh_token,
                    UserId = user.Id,
                    ClientId = request.ClientId,
                    RefreshExpire = client.RefreshTokenExpiresInSeconds
                });

                await Db.SaveChangesAsync();

                return new TokenGenerationResult
                {
                    AuthenticationTicket = token,
                    IsSuccess = true,
                    Message = "Ok",
                    StatusCode = 200
                };
            }
            else if (request.GrantType == GRANT_TYPES.Refresh_Token)
            {
                var tokenDb = Db.Tokens.FirstOrDefault(t => t.RefreshToken == request.RefreshToken);

                bool isValidToken = tokenDb is null
                    || !(Guid.TryParse(request.RefreshToken, out Guid rf))
                    || request.RefreshToken == Guid.Empty.ToString();

                if (isValidToken)
                    return TokenGenerationResult.Fail(400, "Invalid RefreshToken");

                //Simple check -- > I LIKE TO USE IDENTITYSERVER FOR PRODUCTIONS ENVIROMENTS
                if (DateTime.Now.Second >= tokenDb.RefreshExpire)
                    return TokenGenerationResult.Fail(400, "RefreshToken Expired");

                //CREATE A NEW TOKEN 
                tokenDb.Grant = request.GrantType;
                tokenDb.AccessToken = Guid.NewGuid().ToString();
                tokenDb.RefreshToken = Guid.NewGuid().ToString();

                Db.Entry(tokenDb).State = EntityState.Modified;
                
                _ = await Db.SaveChangesAsync();

                return new TokenGenerationResult
                {
                    AuthenticationTicket = new AuthenticationTicketResult
                    {
                        Access_token = tokenDb.AccessToken,
                        Expires_in = tokenDb.ExpireAt,
                        Refresh_token = tokenDb.RefreshToken,
                        Scope = Constants.DEFAULT_SCOPE,
                        Token_type = Constants.TOKEN_TYPE_BR,
                    },
                    IsSuccess = true,
                    StatusCode = 200
                };
            }

            return new TokenGenerationResult { };
        }

        public Task<AuthenticationTicketResult> GenerateTokenByUserAsyn(string username, string password)
        {
            return Task.Run(async () =>
            {
                var user = await Userservice.Authenticate(username, password);

                if (user != null)
                {
                    var token = new AuthenticationTicketResult
                    {
                        Access_token = Guid.NewGuid().ToString(),
                        Expires_in = Convert.ToInt32(TimeSpan.FromSeconds(200).TotalSeconds),
                        Refresh_token = Guid.NewGuid().ToString(),
                        Scope = Constants.DEFAULT_SCOPE,
                        Token_type = Constants.TOKEN_TYPE_BR
                    };

                    await Db.Tokens.AddAsync(new Domain.Entities.Token
                    {
                        AccessToken = token.Access_token,
                        CreateAt = DateTime.Now,
                        ExpireAt = token.Expires_in,
                        GenerateAt = DateTime.Now,
                        Grant = "Code",
                        RefreshToken = token.Refresh_token,
                        UserId = user.Id,
                        ClientId = username
                    });
                    await Db.SaveChangesAsync();
                    return token;
                }
                return null;
            });

        }
    }
}
