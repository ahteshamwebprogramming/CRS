namespace CRS.Core.Repository;

public interface IUnitOfWork
{
    public IRoomTypeRepository RoomType { get; }
}
