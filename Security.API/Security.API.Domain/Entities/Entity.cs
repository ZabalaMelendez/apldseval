using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Domain.Entities
{
    public abstract class Entity
    {
        public virtual int Id { get; set; }
        public virtual DateTime CreateAt { get; set; }

        public abstract bool IsValid();
    }
}
