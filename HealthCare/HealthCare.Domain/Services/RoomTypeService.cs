using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class RoomTypeService : IRoomTypeService{
    private IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository roomTypeRepository) {
        _roomTypeRepository = roomTypeRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<RoomTypeDomainModel>> GetAll()
    {
        var data = await _roomTypeRepository.GetAll();
        if (data == null)
            return null;

        List<RoomTypeDomainModel> results = new List<RoomTypeDomainModel>();
        RoomTypeDomainModel roomTypeModel;
        foreach (var item in data)
        {
            roomTypeModel = new RoomTypeDomainModel
            {
                isDeleted = item.isDeleted,
                Id = item.Id,
                RoleName = item.RoleName,
                Purpose = item.Purpose,
            };
            results.Add(roomTypeModel);
        }

        return results;
    } 
    public async Task<IEnumerable<RoomTypeDomainModel>> ReadAll()
    {
        IEnumerable<RoomTypeDomainModel> roomTypes = await GetAll();
        List<RoomTypeDomainModel> result = new List<RoomTypeDomainModel>();
        foreach (var item in roomTypes)
        {
            if(!item.isDeleted) result.Add(item);
        }
        return result;
    } 
}