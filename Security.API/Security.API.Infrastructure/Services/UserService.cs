using Security.API.Application.DTOS;
using Security.API.Application.Interfaces;
using Security.API.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Security.API.Application.Factories;
using System.Linq;
using Security.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Security.API.Infrastructure.Services
{
    public class UserService : ServiceBase, IUserService
    {
        public UserService(SecurityApiDbContext db)
        {
            Db = db;
        }

        /// <summary>
        /// Get all role for a given username
        /// </summary>
        /// <param name="username">username to check</param>
        /// <returns></returns>
        public async Task<List<string>> GetUserRolesAsyn(string username)
        {
            var user = await Db.Users.FirstOrDefaultAsync(u => u.Username == username);

            var roles = await Task.FromResult(Db.UserRoles
               .Join(Db.Roles, r => r.RoleId, t => t.Id, (ru, rr) => new
               {
                   rr.Name,
                   rr.Id,
                   ru.UserId
               }).Where(x => x.UserId == user.Id).Select(ro => ro.Name).ToList());

            return roles;
        }

        /// <summary>
        /// Get user info and it's roles
        /// </summary>
        /// <param name="username">username to check again db</param>
        /// <returns></returns>
        public async Task<UserDTO> GetUserByNameAsync(string username)
        {
            var user = await Db.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user is null)
                return default;

            return new UserDTO
            {
                Password = user.Password,
                Id = user.Id,
                Username = user.Username,
                Roles = await GetUserRolesAsyn(username)
            };
        }

        public async Task<UserDTO> GetUserByIdAsync(int Id)
        {
            var user = await Db.Users.FirstOrDefaultAsync(u => u.Id == Id);

            if (user is null)
                return default;

            return new UserDTO
            {
                Password = user.Password,
                Id = user.Id,
                Username = user.Username,
                Roles = await GetUserRolesAsyn(user.Username)
            };
        }


        /// <summary>
        /// check if user is in a specify role
        /// </summary>
        /// <param name="userId">id to check <<user>></param>
        /// <param name="roleId">id to check <<role>> </param>
        /// <returns>true/false</returns>
        public async Task<bool> HasRoleAsyn(int userId, int roleId)
        {
            return await Task.FromResult(Db.UserRoles
                  .Include(e => e.Roles)
                  .Include(e => e.User)
                  .Any(d => d.RoleId == roleId && d.UserId == userId));
        }

        /// <summary>
        /// Create user on storage db
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<bool> CreateUserAsync(UserDTO userDto)
        {

            var user = userDto.ToModel();

            if (!userDto.Roles.Any())
            {
                throw new ApplicationException("User don't have a rol, please add one..");
            }

            await Db.Users.AddAsync(user);

            var result = await Db.SaveChangesAsync() > 0;

            userDto.Roles.ForEach(ru =>
            {
                var role = Db.Roles.FirstOrDefault(r => r.Name == ru);
                if (role != null)
                {
                    var userrole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };

                    Db.UserRoles.Add(userrole);
                    Db.SaveChanges();
                }
            });

            return result;
        }


        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserDTO> Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            var user =  Db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user is null)
                return null;

            List<string> roles = null;

            try
            {
                roles = await Task.FromResult(Db.UserRoles
                                  .Join(Db.Roles, r => r.RoleId, t => t.Id, (ru, rr) => new
                                  {
                                      rr.Name,
                                      rr.Id,
                                      ru.UserId
                                  }).Where(x => x.UserId == user.Id).Select(ro => ro.Name).ToList());

            }
            catch (Exception)
            {

            }

            return new UserDTO
            {
                Password = user.Password,
                Username = user.Username,
                Roles = roles,
                Id = user.Id
            };
        }
    }
}
