using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Domain.Entities
{
    public partial class User : Entity
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        
        public override bool IsValid()
        {
            throw new NotImplementedException();
        }


        public virtual ICollection<UserRole> UserRoles { get; set; }

        
    }
}
