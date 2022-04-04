using Security.API.Application.DTOS;
using Security.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Security.API.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByIdAsync(int Id);
        Task<List<string>> GetUserRolesAsyn(string username);
        Task<UserDTO> GetUserByNameAsync(string username);
        Task<bool> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> Authenticate(string username, string password);
    }
}
