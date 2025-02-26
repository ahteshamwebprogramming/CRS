using CRS.Core.Entities;
using CRS.Services.DBContext;
using CRS.Core.Repository;

namespace CRS.Services
{
    public class GenOperationsRepository : DapperGenericRepository<GenOperations>, IGenOperationsRepository
    {
        public GenOperationsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {
        }
    }
}
