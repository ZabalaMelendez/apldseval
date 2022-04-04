using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Application.Interfaces
{
    public interface ISecurityApiDbContext
    {
        int SaveChanges();
    }
}
