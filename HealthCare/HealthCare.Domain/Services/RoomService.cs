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

    public async Task<IEnumerable<RoomDomainModel>> ReadAll()
    {
        IEnumerable<RoomDomainModel> rooms = await GetAll();
        List<RoomDomainModel> result = new List<RoomDomainModel>();
        foreach (var item in rooms)
        {
            if(!item.isDeleted) result.Add(item);
        }
        return result;
    }

  public async Task<IEnumerable<RoomDomainModel>> GetAll()
    {
        IEnumerable<Room> rooms = await _roomRepository.GetAll();
        if (rooms == null)
            return null;

        List<RoomDomainModel> results = new List<RoomDomainModel>();
        RoomDomainModel roomModel;
        foreach (Room item in rooms)
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

    public async Task<RoomDomainModel> Add(RoomDomainModel newRoomModel)
    {
        Room newRoom = new Room();
        newRoom.isDeleted = newRoomModel.isDeleted;
        newRoom.RoomName = newRoomModel.RoomName;
        RoomType roomType = await _roomTypeRepository.GetById(newRoomModel.RoomTypeId);
        newRoom.RoomType = roomType;
        newRoom.RoomTypeId = roomType.Id;
        Room insertedRoom = _roomRepository.Post(newRoom);
        _roomRepository.Save();

        return newRoomModel;
    }

    public async Task<RoomDomainModel> Update(RoomDomainModel updatedRoomModel, decimal id)
    {
        Room updatedRoom = await _roomRepository.GetRoomById(id);
        updatedRoom.isDeleted = updatedRoomModel.isDeleted;
        //r.Inventories = room.Inventories;
        //r.Operations = room.Operations;
        updatedRoom.RoomName = updatedRoomModel.RoomName;
        RoomType roomType = await _roomTypeRepository.GetById(updatedRoomModel.RoomTypeId);
        updatedRoom.RoomType = roomType;
        updatedRoom.RoomTypeId = roomType.Id;
        //r.RoomTypeId = room.RoomType;
        _roomRepository.Update(updatedRoom);
        _roomRepository.Save();

        return updatedRoomModel;


    }

    public async Task<RoomDomainModel> Delete(decimal id)
    {
        Room deletedRoom = await _roomRepository.GetRoomById(id);
        deletedRoom.isDeleted = true;
        _ = _roomRepository.Update(deletedRoom);
        _roomRepository.Save();
        return parseToModel(deletedRoom);
    }

    private RoomDomainModel parseToModel(Room deletedRoom)
    {
        return new RoomDomainModel
        {
            Id = deletedRoom.Id,
            RoomName = deletedRoom.RoomName,
            RoomTypeId = deletedRoom.RoomTypeId,
            isDeleted = deletedRoom.isDeleted

        };
        return new RoomDomainModel();
    }
}