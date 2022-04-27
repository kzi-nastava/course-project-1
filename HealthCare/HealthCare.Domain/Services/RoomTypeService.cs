using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class RoomTypeService : IRoomTypeService{
    private IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository roomTypeRepository) {
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<IEnumerable<RoomTypeDomainModel>> GetAll()
    {
        IEnumerable<RoomType> roomTypes = await _roomTypeRepository.GetAll();
        if (roomTypes == null)
            return null;

        List<RoomTypeDomainModel> results = new List<RoomTypeDomainModel>();
        RoomTypeDomainModel roomTypeModel;
        foreach (RoomType item in roomTypes)
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
}