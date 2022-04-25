using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IRoomService : IService<RoomDomainModel>
{
    Task<RoomDomainModel> Add(RoomDomainModel room);
    Task<RoomDomainModel> Update(RoomDomainModel room, decimal id);
    Task<RoomDomainModel> Delete(decimal id);
}