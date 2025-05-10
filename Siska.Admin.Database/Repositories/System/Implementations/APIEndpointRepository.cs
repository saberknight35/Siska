using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Database.Repositories.System.Implementations
{
    public class APIEndpointRepository : Repository<APIEndpoint>, IAPIEndpointRepository
    {
        public APIEndpointRepository(SiskaDbContext dbContext) : base(dbContext, SpecificationEvaluator.Default)
        {
        }

        public APIEndpointRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
        {
        }

    }
}
