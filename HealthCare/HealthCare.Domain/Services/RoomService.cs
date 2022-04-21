using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class RoomService : IRoomService{
    private IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository) {
        _roomRepository = roomRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<RoomDomainModel>> GetAll()
    {
        var data = await _roomRepository.GetAll();
        if (data == null)
            return null;

        List<RoomDomainModel> results = new List<RoomDomainModel>();
        RoomDomainModel roomModel;
        foreach (var item in data)
        {
            roomModel = new RoomDomainModel
            {
                isDeleted = item.isDeleted,
                Id = item.Id,
                Inventories = item.Inventories,
                Operations = item.Operations,
                RoleName = item.RoleName,
                RoomType = item.RoomType,
                RoomTypeId = item.RoomTypeId,
                TransfersFrom = item.TransfersFrom,
                TransfersTo = item.TransfersTo
            };
            results.Add(roomModel);
        }

        return results;
    } 
}