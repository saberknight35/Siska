using Riok.Mapperly.Abstractions;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Model.Mappings
{
    [Mapper]
    public static partial class RoleMapping
    {
        public static partial RoleDTO EntitiesToDTO(Roles entity);
        public static partial List<RoleDTO> ListEntitiesToDTO(List<Roles> entity);
        public static partial Roles DTOtoEntities(RoleDTO dto);

        // public static partial TTarget  Parse<TTarget>(string value);
    }
}