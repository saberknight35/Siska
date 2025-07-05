using Siska.Admin.Model.DTO.Master;
using Siska.Admin.Model.Entities.Master;

namespace Siska.Admin.Model.Mappings.Master
{
    public static class AdministrativeAreaMappings
    {
        // Province mappings
        public static ProvinceDto ToDto(this Province entity, bool includeChildren = false)
        {
            var dto = new ProvinceDto
            {
                Id = entity.Id,
                ProvinceCode = entity.ProvinceCode,
                ProvinceName = entity.ProvinceName,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                ModifiedBy = entity.ModifiedBy,
                ModifiedDate = entity.ModifiedDate
            };

            if (includeChildren && entity.RegenciesCities != null)
            {
                dto.RegenciesCities = entity.RegenciesCities.Select(r => r.ToDto()).ToList();
            }

            return dto;
        }

        public static Province ToEntity(this ProvinceDto dto)
        {
            return new Province
            {
                Id = dto.Id,
                ProvinceCode = dto.ProvinceCode,
                ProvinceName = dto.ProvinceName,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                ModifiedBy = dto.ModifiedBy,
                ModifiedDate = dto.ModifiedDate
            };
        }

        // RegencyCity mappings
        public static RegencyCityDto ToDto(this RegencyCity entity, bool includeChildren = false)
        {
            var dto = new RegencyCityDto
            {
                Id = entity.Id,
                RegencyCode = entity.RegencyCode,
                RegencyName = entity.RegencyName,
                Type = entity.Type,
                ProvinceId = entity.ProvinceId,
                ProvinceName = entity.Province?.ProvinceName,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                ModifiedBy = entity.ModifiedBy,
                ModifiedDate = entity.ModifiedDate
            };

            if (includeChildren && entity.Districts != null)
            {
                dto.Districts = entity.Districts.Select(d => d.ToDto()).ToList();
            }

            return dto;
        }

        public static RegencyCity ToEntity(this RegencyCityDto dto)
        {
            return new RegencyCity
            {
                Id = dto.Id,
                RegencyCode = dto.RegencyCode,
                RegencyName = dto.RegencyName,
                Type = dto.Type,
                ProvinceId = dto.ProvinceId,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                ModifiedBy = dto.ModifiedBy,
                ModifiedDate = dto.ModifiedDate
            };
        }

        // District mappings
        public static DistrictDto ToDto(this District entity, bool includeChildren = false)
        {
            var dto = new DistrictDto
            {
                Id = entity.Id,
                DistrictCode = entity.DistrictCode,
                DistrictName = entity.DistrictName,
                RegencyCityId = entity.RegencyCityId,
                RegencyCityName = entity.RegencyCity?.RegencyName,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                ModifiedBy = entity.ModifiedBy,
                ModifiedDate = entity.ModifiedDate
            };

            if (includeChildren && entity.Villages != null)
            {
                dto.Villages = entity.Villages.Select(v => v.ToDto()).ToList();
            }

            return dto;
        }

        public static District ToEntity(this DistrictDto dto)
        {
            return new District
            {
                Id = dto.Id,
                DistrictCode = dto.DistrictCode,
                DistrictName = dto.DistrictName,
                RegencyCityId = dto.RegencyCityId,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                ModifiedBy = dto.ModifiedBy,
                ModifiedDate = dto.ModifiedDate
            };
        }

        // Village mappings
        public static VillageDto ToDto(this Village entity)
        {
            return new VillageDto
            {
                Id = entity.Id,
                VillageCode = entity.VillageCode,
                VillageName = entity.VillageName,
                Type = entity.Type,
                DistrictId = entity.DistrictId,
                DistrictName = entity.District?.DistrictName,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                ModifiedBy = entity.ModifiedBy,
                ModifiedDate = entity.ModifiedDate
            };
        }

        public static Village ToEntity(this VillageDto dto)
        {
            return new Village
            {
                Id = dto.Id,
                VillageCode = dto.VillageCode,
                VillageName = dto.VillageName,
                Type = dto.Type,
                DistrictId = dto.DistrictId,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                ModifiedBy = dto.ModifiedBy,
                ModifiedDate = dto.ModifiedDate
            };
        }

        // List extensions
        public static List<ProvinceDto> ToDtoList(this IEnumerable<Province> entities, bool includeChildren = false)
        {
            return entities.Select(e => e.ToDto(includeChildren)).ToList();
        }

        public static List<RegencyCityDto> ToDtoList(this IEnumerable<RegencyCity> entities, bool includeChildren = false)
        {
            return entities.Select(e => e.ToDto(includeChildren)).ToList();
        }

        public static List<DistrictDto> ToDtoList(this IEnumerable<District> entities, bool includeChildren = false)
        {
            return entities.Select(e => e.ToDto(includeChildren)).ToList();
        }

        public static List<VillageDto> ToDtoList(this IEnumerable<Village> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }
    }
}