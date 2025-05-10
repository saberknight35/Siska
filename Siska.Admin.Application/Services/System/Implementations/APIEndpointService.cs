using Microsoft.AspNetCore.Identity;
using Siska.Admin.Application.Constants;
using Siska.Admin.Application.Enums;
using Siska.Admin.Application.Exceptions;
using Siska.Admin.Cache;
using Siska.Admin.Database;
using Siska.Admin.Database.Repositories.System;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Entities;
using Siska.Admin.Model.Extensions;
using Siska.Admin.Model.Mappings;
using Siska.Admin.Utility;

namespace Siska.Admin.Application.Services.System.Implementations
{
    public class APIEndpointService : IAPIEndpointService
    {
        private IAPIEndpointRepository apiEndpointRepository;
        private RoleManager<Roles> roles;
        private ICacheService cacheService;
        private IExtractUser extractUser;

        public APIEndpointService(
            IAPIEndpointRepository apiEndpointRepository,
            RoleManager<Roles> roles,
            ICacheService cacheService,
            IExtractUser extractUser)
        {
            this.apiEndpointRepository = apiEndpointRepository;
            this.roles = roles;
            this.cacheService = cacheService;
            this.extractUser = extractUser;
        }

        public async Task<APIEndpointDTO> Add(APIEndpointDTO apiEndpointDTO, CancellationToken cancellationToken)
        {
            apiEndpointDTO.Validate();

            for (int i = 0; i < apiEndpointDTO.Roles.Count; i++)
            {
                var role = apiEndpointDTO.Roles[i];

                var dataRole = await roles.FindByIdAsync(role.Id.ToString());
                NotFoundException.ThrowIfNull(dataRole, IPSEntityType.Roles);

                apiEndpointDTO.Roles[i] = dataRole;
            }

            var data = APIEndpointMapping.DTOtoEntities(apiEndpointDTO);

            data.CreatedDate = DateTime.UtcNow;
            data.CreatedBy = extractUser.Username;
            data.ModifiedDate = DateTime.UtcNow;
            data.ModifiedBy = extractUser.Username;
            data.ApiPath = apiEndpointDTO.ApiPath.ToLower();
            data.ApiMethod = apiEndpointDTO.ApiMethod.ToUpper();

            var result = await apiEndpointRepository.AddAsync(data, cancellationToken);

            if (result == null)
            {
                throw new Exception("Failed to add apiEndpoint");
            }

            await RefreshCache();

            return APIEndpointMapping.EntitiesToDTO(result);
        }

        public async Task<bool> Delete(int id, CancellationToken cancellationToken)
        {
            var data = await apiEndpointRepository.GetByIdAsync(id, cancellationToken);

            NotFoundException.ThrowIfNull(data, IPSEntityType.APIEndpoint);

            if (data != null)
            {
                await apiEndpointRepository.DeleteAsync(data, cancellationToken);

                await RefreshCache();
                return true;
            }

            return false;
        }

        public async Task<APIEndpointDTO> Get(int id, CancellationToken cancellationToken)
        {
            var cacheKey = await GetCache();

            var data = cacheKey.FirstOrDefault(q => q.Id == id);

            NotFoundException.ThrowIfNull(data, IPSEntityType.APIEndpoint);

            return APIEndpointMapping.EntitiesToDTO(data);
        }

        public async Task<PagedList<APIEndpointDTO>> GetList(ListDataDTO listDataDTO, CancellationToken cancellationToken)
        {
            var result = await apiEndpointRepository.GetPagedList(listDataDTO, null, cancellationToken);

            var data = new PagedList<APIEndpointDTO>(APIEndpointMapping.ListEntitiesToDTO(result.Data.ToList()), result.TotalItems, result.PageNumber, listDataDTO.PageSize);

            return data;
        }

        public async Task<bool> Update(APIEndpointDTO apiEndpointDTO, CancellationToken cancellationToken)
        {
            apiEndpointDTO.Validate();

            var data = await apiEndpointRepository.FirstOrDefaultAsync(q => q.Id == apiEndpointDTO.Id, q => q.Include(r => r.Roles), cancellationToken);

            NotFoundException.ThrowIfNull(data, IPSEntityType.APIEndpoint);

            if (data != null)
            {
                data.ModifiedDate = DateTime.UtcNow;
                data.ModifiedBy = extractUser.Username;
                data.ApiDescription = apiEndpointDTO.ApiDescription;

                data.Roles.Clear();

                for (int i = 0; i < apiEndpointDTO.Roles.Count; i++)
                {
                    var role = apiEndpointDTO.Roles[i];

                    var dataRole = await roles.FindByIdAsync(role.Id.ToString());
                    NotFoundException.ThrowIfNull(dataRole, IPSEntityType.Roles);

                    apiEndpointDTO.Roles[i] = dataRole;
                }

                data.Roles = apiEndpointDTO.Roles;

                await apiEndpointRepository.UpdateAsync(data, cancellationToken);

                await RefreshCache();

                return true;
            }

            return false;
        }

        public async Task<bool> IsMatch(string apiPath, string apiMethod, List<string> userRoles)
        {
            var cacheKey = await GetCache();

            var result = cacheKey.SingleOrDefault(q => q.ApiPath == apiPath.ToLower() && q.ApiMethod == apiMethod);

            if (result == null)
            {
                return false;
            }

            //Anonymous
            if (result.Roles == null)
            {
                return true;
            }

            var roles = result.Roles.Select(s => s.Name).ToList();

            var common = roles.Intersect(userRoles);

            return common.Any();
        }

        private async Task RefreshCache()
        {
            var cacheKey = await cacheService.GetAsync<List<APIEndpoint>>(Caches.APIEndpoint);

            if (cacheKey != null)
            {
                cacheKey.Clear();
            }

            cacheKey = (await apiEndpointRepository.ListAsync(spec => spec.Include(i => i.Roles), new CancellationToken())).ToList();

            await cacheService.SetAsync(Caches.APIEndpoint, cacheKey);
        }

        private async Task<List<APIEndpoint>> GetCache()
        {
            var cacheKey = await cacheService.GetAsync<List<APIEndpoint>>(Caches.APIEndpoint);

            if (cacheKey == null)
            {
                cacheKey = (await apiEndpointRepository.ListAsync(spec => spec.Include(i => i.Roles))).ToList();

                await cacheService.SetAsync(Caches.APIEndpoint, cacheKey);
            }

            return cacheKey;
        }
    }
}
