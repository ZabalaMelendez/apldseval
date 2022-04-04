using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Domain.Entities
{
    public class Token : Entity
    {
        public DateTime GenerateAt { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpireAt { get; set; }
        public string Grant { get; set; }
        public int UserId { get; set; }
        public string ClientId { get; set; }
        public int RefreshExpire { get; set; }
        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
