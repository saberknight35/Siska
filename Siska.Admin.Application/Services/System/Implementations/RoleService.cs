using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Siska.Admin.Application.Enums;
using Siska.Admin.Application.Exceptions;
using Siska.Admin.Database;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Entities;
using Siska.Admin.Model.Extensions;
using Siska.Admin.Model.Mappings;
using Siska.Admin.Utility;

namespace Siska.Admin.Application.Services.System.Implementations
{
    public class RoleService : IRoleService
    {
        private RoleManager<Roles> roles;

        public RoleService(RoleManager<Roles> roles)
        {
            this.roles = roles;
        }

        #region Main CRUD
        public async Task<RoleDTO> Add(RoleDTO roleDTO, CancellationToken cancellationToken)
        {
            roleDTO.Validate();

            if (roleDTO.Name == "Admin")
                throw new Exception("Sorry can not add 'Admin' role");

            roleDTO.Id = Guid.NewGuid().ToString();

            var role = RoleMapping.DTOtoEntities(roleDTO);

            await roles.CreateAsync(role);

            return RoleMapping.EntitiesToDTO(role);
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
        {
            var data = await roles.Roles.FirstAsync(x => x.Id == id);

            NotFoundException.ThrowIfNull(data, IPSEntityType.Roles);

            if (data != null)
            {
                if (data.Name == "Admin")
                    throw new Exception("Sorry can not delete 'Admin' role");

                var result = await roles.DeleteAsync(data);
            }

            return false;
        }

        public async Task<RoleDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var data = await roles.Roles.FirstAsync(x => x.Id == id, cancellationToken);

            NotFoundException.ThrowIfNull(data, IPSEntityType.Roles);

            var dataDTO = RoleMapping.EntitiesToDTO(data);

            return dataDTO;
        }

        public async Task<PagedList<RoleDTO>> GetList(ListDataDTO listDataDTO, CancellationToken cancellationToken)
        {
            if (listDataDTO.PageNumber <= 0) listDataDTO.PageNumber = 1;

            if (listDataDTO.PageSize <= 0) listDataDTO.PageSize = 10;
            var skipSize = (listDataDTO.PageNumber - 1) * listDataDTO.PageSize;

            int totalItems = 0;
            List<RoleDTO> data = null;

            if (listDataDTO.Search != null && listDataDTO.Search.Count > 0)
            {
                Expression<Func<Roles, bool>> exp;

                exp = ExpressionUtils.BuildCondition<Roles>(listDataDTO.Search);

                totalItems = await roles.Roles.CountAsync(exp, cancellationToken: cancellationToken);
                data = RoleMapping.ListEntitiesToDTO(await roles.Roles.Where(exp).Skip(skipSize).Take(listDataDTO.PageSize).ToListAsync());
            }
            else if (!listDataDTO.SearchField.IsNullOrEmpty() && !listDataDTO.SearchString.IsNullOrEmpty())
            {
                Expression<Func<Roles, bool>> exp;

                exp = ExpressionUtils.BuildPredicate<Roles>(listDataDTO.SearchField, "==", listDataDTO.SearchString);

                totalItems = await roles.Roles.CountAsync(exp, cancellationToken: cancellationToken);
                data = RoleMapping.ListEntitiesToDTO(await roles.Roles.Where(exp).Skip(skipSize).Take(listDataDTO.PageSize).ToListAsync());
            }
            else
            {
                totalItems = await roles.Roles.CountAsync(cancellationToken: cancellationToken);
                data = RoleMapping.ListEntitiesToDTO(await roles.Roles.Skip(skipSize).Take(listDataDTO.PageSize).ToListAsync());
            }

            return new PagedList<RoleDTO>(data, totalItems, listDataDTO.PageNumber, listDataDTO.PageSize);
        }

        public async Task<bool> Update(RoleDTO roleDTO, CancellationToken cancellationToken)
        {
            roleDTO.Validate();

            if (roleDTO.Name == "Admin")
                throw new Exception("Sorry can not change 'Admin' role");

            var Role = RoleMapping.DTOtoEntities(roleDTO);

            var data = await roles.Roles.FirstAsync(x => x.Id == Role.Id);

            NotFoundException.ThrowIfNull(data, IPSEntityType.Roles);

            if (data != null)
            {
                data.Description = Role.Description;

                var result = await roles.UpdateAsync(data);

                if (result != null)
                {
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    return true;
                }
            }

            return false;
        }
        #endregion Main CRUD
    }
}
