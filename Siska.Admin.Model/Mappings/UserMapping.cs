using Riok.Mapperly.Abstractions;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Model.Mappings
{
    [Mapper]
    public static partial class UserMapping
    {
        public static partial UserDTO EntitiesToDTO(Users entity);
        public static partial List<UserDTO> ListEntitiesToDTO(List<Users> entity);
        public static partial Users DTOtoEntities(UserDTO dto);
    }
}