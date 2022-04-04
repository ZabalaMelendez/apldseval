using Microsoft.EntityFrameworkCore;
using Security.API.Application.Interfaces;
using Security.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Infrastructure.Persistence
{

    public partial class SecurityApiDbContext : DbContext, ISecurityApiDbContext
    {
        public SecurityApiDbContext(DbContextOptions<SecurityApiDbContext> options)
            : base(options: options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
    }
}
