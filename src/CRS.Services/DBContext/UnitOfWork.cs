using AutoMapper;
using CRS.Core.Entities;
using CRS.Core.Repository;

namespace CRS.Services.DBContext;

public class UnitOfWork : IUnitOfWork
{
    private readonly DapperDBContext _dapperDBContext;
    private readonly DapperEHRMSDBContext _dapperEHRMSDBContext;
    public UnitOfWork(DapperDBContext dapperDBContext, DapperEHRMSDBContext dapperEHRMSDBContext, IMapper mapper) {
        _dapperDBContext = dapperDBContext;
        _dapperEHRMSDBContext = dapperEHRMSDBContext;
        GenOperations = new GenOperationsRepository(_dapperDBContext);
        RoomType = new RoomTypeRepository(_dapperDBContext);
    }
    public IGenOperationsRepository GenOperations { get; private set; }
    public IRoomTypeRepository RoomType { get; private set; }
}
