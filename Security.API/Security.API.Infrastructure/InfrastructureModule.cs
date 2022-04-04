using Microsoft.Extensions.DependencyInjection;
using Security.API.Infrastructure.Persistence;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Security.API.Domain.Entities;
using System.Linq;
using Security.API.Infrastructure.Services;
using Security.API.Application.Interfaces;

namespace Security.API.Infrastructure
{
    public static class InfrastructureModule
    {
        #region Extensions

        /// <summary>
        /// Inject infra in to DI
        /// </summary>
        /// <param name="services">service collection from builtIn DI</param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructureSecurity(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services), "Application not started...");

            services.AddDbContext<SecurityApiDbContext>(opDb => opDb.UseInMemoryDatabase("Securityapi"));

            services.AddScoped<IClientsService, ClientService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            AddDefaultData(services);

            return services;
        }

        #endregion

        #region Methods
        private static IServiceCollection AddDefaultData(IServiceCollection service)
        {
            var client = new Client
            {
                AccessTokenExpiresInSeconds = 60,
                ClientId = "applaudostudios",
                ClientSecred = "mp6WF46YZNaMFzfQ",
                CreateAt = DateTime.Now,
                Name = "applaudostudios",
                RedirectUri = "https://applaudostudios.com/",
                RefreshTokenExpiresInSeconds = 60,
                Scopes = "any"
            };

            var roleAdmin = new Role
            {
                Name = "ADMIN",
                CreateAt = DateTime.Now,
            };


            var roleUser = new Role
            {
                Name = "USER",
                CreateAt = DateTime.Now,
            };


            var roleGuest = new Role
            {
                Name = "GUEST",
                CreateAt = DateTime.Now,
            };

            var db = service.BuildServiceProvider().GetService<SecurityApiDbContext>();

            if (db != null)
            {
                if (!db.Clients.Any(c => c.ClientId == client.ClientId))
                {
                    db.Clients.Add(client);
                    db.SaveChanges();
                }

                //HACK: <<IFNOTINMEMORY>>
                var roles = db.Roles.ToList();
                db.Roles.RemoveRange(roles);
                db.SaveChanges();

                db.Roles.Add(roleAdmin);
                db.Roles.Add(roleUser);
                db.Roles.Add(roleGuest);

                db.SaveChanges();

            }

            return service;
        }
        #endregion
    }
}
