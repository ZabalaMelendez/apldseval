using Security.API.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Infrastructure.Services
{
    public class ServiceBase
    {
        protected virtual SecurityApiDbContext Db { get; set; }

    }
}
