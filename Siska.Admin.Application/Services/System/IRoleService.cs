using Siska.Admin.Database;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;

namespace Siska.Admin.Application.Services.System
{
    public interface IRoleService
    {
        Task<PagedList<RoleDTO>> GetList(ListDataDTO listDataDTO, CancellationToken cancellationToken);
        Task<RoleDTO> Get(Guid id, CancellationToken cancellationToken);
        Task<RoleDTO> Add(RoleDTO userDTO, CancellationToken cancellationToken);
        Task<bool> Update(RoleDTO userDTO, CancellationToken cancellationToken);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken);
    }
}
