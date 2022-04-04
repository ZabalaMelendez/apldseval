using Security.API.Application.DTOS;
using Security.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Application.Factories
{
    public static class UserExtensionFactory
    {

        public static User ToModel(this UserDTO dto)
            => new User
            {
                CreateAt = DateTime.Now,
                Password = dto.Password,
                Username = dto.Username
            };

    }
}
