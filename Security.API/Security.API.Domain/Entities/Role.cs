using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Domain.Entities
{
    public class Role : Entity
    {
        public string Name { get; set; }
        public Role()
        {
        }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }

        public virtual UserRole UserRole { get; set; }
    }

    public class UserRole : Entity
    {
        public UserRole()
        {
            Roles = new HashSet<Role>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        public virtual User User{ get; set; }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
