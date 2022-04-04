using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Security.API.Application.DTOS
{
    public class UserDTO
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }

        public List<string> Roles { get; set; }
    }
}
