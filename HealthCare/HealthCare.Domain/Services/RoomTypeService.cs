using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class RoomTypeService : IRoomTypeService{
    private IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository roomTypeRepository) {
        _roomTypeRepository = roomTypeRepository;
    }

    public Task<IEnumerable<RoomTypeDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}