using Riok.Mapperly.Abstractions;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Model.Mappings
{
    [Mapper]
    public static partial class APIEndpointMapping
    {
        public static partial APIEndpointDTO EntitiesToDTO(APIEndpoint entity);
        public static partial List<APIEndpointDTO> ListEntitiesToDTO(List<APIEndpoint> entity);
        public static partial APIEndpoint DTOtoEntities(APIEndpointDTO dto);
    }
}
