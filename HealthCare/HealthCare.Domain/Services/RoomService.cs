using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class RoomService : IRoomService{
    private IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository) {
        _roomRepository = roomRepository;
    }

    public Task<IEnumerable<RoomDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}