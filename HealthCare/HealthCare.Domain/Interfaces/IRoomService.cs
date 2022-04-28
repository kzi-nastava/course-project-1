using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IRoomService : IService<RoomDomainModel>
{
    public Task<RoomDomainModel> Add(RoomDomainModel roomModel);
    public Task<RoomDomainModel> Update(RoomDomainModel roomModel, decimal id);
    public Task<RoomDomainModel> Delete(decimal id);
    public Task<IEnumerable<RoomDomainModel>> ReadAll();
}