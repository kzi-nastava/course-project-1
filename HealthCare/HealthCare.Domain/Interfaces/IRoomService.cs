using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IRoomService : IService<RoomDomainModel>
{
    public Task<RoomDomainModel> Create(CURoomDTO dto);
    public Task<RoomDomainModel> Update(CURoomDTO dto);
    public Task<RoomDomainModel> Delete(decimal id);
    public Task<decimal> GetAvailableRoomId(DateTime startTime);
    public Task<IEnumerable<RoomDomainModel>> ReadAll();
}