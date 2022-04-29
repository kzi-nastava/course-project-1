using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
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
        foreach (RoomDomainModel item in rooms)
        {
            if (!item.IsDeleted) result.Add(item);
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
                IsDeleted = item.IsDeleted,
                Id = item.Id,
                RoomName = item.RoomName,
                RoomTypeId = item.RoomTypeId,
            };
            if(item.RoomType != null) {
                roomModel.RoomType = new RoomTypeDomainModel {
                    IsDeleted = item.RoomType.IsDeleted,
                    Id = item.RoomType.Id,
                    RoleName = item.RoomType.RoleName,
                    Purpose = item.RoomType.Purpose,
                };
            }
            roomModel.Inventories = new List<InventoryDomainModel>();
            roomModel.Operations = new List<OperationDomainModel>();
            if (item.Inventories != null) {
                foreach (Inventory inventory in item.Inventories) {
                    InventoryDomainModel inventoryModel = new InventoryDomainModel {
                        IsDeleted = inventory.IsDeleted,
                        RoomId = inventory.RoomId,
                        Amount = inventory.Amount,
                        EquipmentId = inventory.RquipmentId,
                    };
                    inventoryModel.Equipment = new EquipmentDomainModel {
                        Id = inventory.Equipment.Id,
                        EquipmentTypeId = inventory.Equipment.equipmentTypeId,
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
                foreach (Operation operation in item.Operations) {
                    OperationDomainModel operationModel = new OperationDomainModel {
                        DoctorId = operation.DoctorId,
                        RoomId = operation.DoctorId,
                        PatientId = operation.DoctorId,
                        Duration = operation.Duration,
                        IsDeleted = operation.IsDeleted
                    };
                    roomModel.Operations.Add(operationModel);
                }
            }
            results.Add(roomModel);
        }
       

        return results;
    }

    public async Task<RoomDomainModel> Add(RoomDomainModel roomModel)
    {
        Room newRoom = new Room();
        newRoom.IsDeleted = roomModel.IsDeleted;
        newRoom.RoomName = roomModel.RoomName;
        RoomType roomType = await _roomTypeRepository.GetById(roomModel.RoomTypeId);
        newRoom.RoomType = roomType;
        newRoom.RoomTypeId = roomType.Id;
        _ = _roomRepository.Post(newRoom);
        _roomRepository.Save();

        return roomModel;
    }

    public async Task<RoomDomainModel> Update(RoomDomainModel roomModel)
    {
        Room room = await _roomRepository.GetRoomById(roomModel.Id);
        room.IsDeleted = roomModel.IsDeleted;
        room.RoomName = roomModel.RoomName;
        RoomType roomType = await _roomTypeRepository.GetById(roomModel.RoomTypeId);
        room.RoomType = roomType;
        room.RoomTypeId = roomType.Id;
        _ = _roomRepository.Update(room);
        _roomRepository.Save();

        return roomModel;
    }

    public async Task<RoomDomainModel> Delete(decimal id)
    {
        Room deletedRoom = await _roomRepository.GetRoomById(id);
        deletedRoom.IsDeleted = true;
        _ = _roomRepository.Update(deletedRoom);
        _roomRepository.Save();
        return parseToModel(deletedRoom);
    }

    private RoomDomainModel parseToModel(Room room)
    {
        RoomDomainModel roomModel = new RoomDomainModel {
            Id = room.Id,
            RoomName = room.RoomName,
            RoomTypeId = room.RoomTypeId,
            IsDeleted = room.IsDeleted
        };
        return roomModel;
    }
}