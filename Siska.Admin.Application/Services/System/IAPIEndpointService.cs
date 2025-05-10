using Siska.Admin.Database;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;

namespace Siska.Admin.Application.Services.System
{
    public interface IAPIEndpointService
    {
        Task<PagedList<APIEndpointDTO>> GetList(ListDataDTO listDataDTO, CancellationToken cancellationToken);
        Task<APIEndpointDTO> Get(int id, CancellationToken cancellationToken);
        Task<APIEndpointDTO> Add(APIEndpointDTO districtDTO, CancellationToken cancellationToken);
        Task<bool> Update(APIEndpointDTO districtDTO, CancellationToken cancellationToken);
        Task<bool> Delete(int id, CancellationToken cancellationToken);
        Task<bool> IsMatch(string apiPath, string apiMethod, List<string> userRoles);
    }
}
