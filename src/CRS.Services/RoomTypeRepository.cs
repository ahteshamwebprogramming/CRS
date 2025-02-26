using CRS.Core.Entities;
using CRS.Core.Repository;
using CRS.Services.DBContext;

namespace CRS.Services;

public class RoomTypeRepository : DapperGenericRepository<RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}