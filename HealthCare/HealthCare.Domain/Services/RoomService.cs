using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForUpdate;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class RoomService : IRoomService{
    private IRoomRepository _roomRepository;
    private IRoomTypeRepository _roomTypeRepository;

    public RoomService(IRoomRepository roomRepository, IRoomTypeRepository roomTypeRepository) {
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
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
                RoomName = item.RoomName,
                RoomTypeId = item.RoomTypeId,
            };
            if(item.RoomType != null) {
                roomModel.RoomType = new RoomTypeDomainModel {
                    isDeleted = item.RoomType.isDeleted,
                    Id = item.RoomType.Id,
                    RoleName = item.RoomType.RoleName,
                    Purpose = item.RoomType.Purpose,
                };
            }
            roomModel.Inventories = new List<InventoryDomainModel>();
            roomModel.Operations = new List<OperationDomainModel>();
            if (item.Inventories != null) {
                foreach (var inventory in item.Inventories) {
                    InventoryDomainModel inventoryModel = new InventoryDomainModel {
                        IsDeleted = inventory.IsDeleted,
                        roomId = inventory.roomId,
                        Amount = inventory.Amount,
                        equipmentId = inventory.equipmentId,
                    };
                    inventoryModel.Equipment = new EquipmentDomainModel {
                        Id = inventory.Equipment.Id,
                        equipmentTypeId = inventory.Equipment.equipmentTypeId,
                        IsDeleted = inventory.Equipment.IsDeleted,
                        Name = inventory.Equipment.Name,
                    };
                    if (inventoryModel.Equipment.EquipmentType != null)
                        inventoryModel.Equipment.EquipmentType = new EquipmentTypeDomainModel {
                            Id = inventory.Equipment.EquipmentType.Id,
                            Name = inventory.Equipment.EquipmentType.Name,
                            IsDeleted = inventory.Equipment.EquipmentType.IsDeleted
                        };
                    roomModel.Inventories.Add(inventoryModel);
                }
            }
            if (item.Operations != null) {
                foreach (var operation in item.Operations) {
                    OperationDomainModel operationDomainModel = new OperationDomainModel {
                        DoctorId = operation.DoctorId,
                        RoomId = operation.DoctorId,
                        PatientId = operation.DoctorId,
                        Duration = operation.Duration,
                        isDeleted = operation.isDeleted
                    };
                    roomModel.Operations.Add(operationDomainModel);
                }
            }
            results.Add(roomModel);
        }
       

        return results;
    }

    public async Task<RoomDomainModel> Add(RoomDomainModel room)
    {
        Room r = new Room();
        r.isDeleted = room.isDeleted;
        r.RoomName = room.RoomName;
        RoomType roomType = await _roomTypeRepository.GetById(room.RoomTypeId);
        r.RoomType = roomType;
        r.RoomTypeId = roomType.Id;
        Room insertedRoom = _roomRepository.Post(r);
        _roomRepository.Save();

        return room;
    }

    public async Task<RoomDomainModel> Update(RoomDomainModel room, decimal id)
    {
        Room r = await _roomRepository.GetRoomById(id);
        r.isDeleted = room.isDeleted;
        //r.Inventories = room.Inventories;
        //r.Operations = room.Operations;
        r.RoomName = room.RoomName;
        RoomType roomType = await _roomTypeRepository.GetById(room.RoomTypeId);
        r.RoomType = roomType;
        r.RoomTypeId = roomType.Id;
        //r.RoomTypeId = room.RoomType;
        _ = _roomRepository.Update(r);
        _roomRepository.Save();

        return r;


    }

    public async Task<RoomDomainModel> Delete(decimal id)
    {
        Room r = await _roomRepository.GetRoomById(id);
        r.isDeleted = true;
        _ = _roomRepository.Update(r);
        _roomRepository.Save();
        return null;
    }
}