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
                RoleName = item.RoleName,
                RoomTypeId = item.RoomTypeId,
            };
            if(item.RoomType != null) {
                roomModel.RoomType = new RoomTypeDomainModel {
                    isDeleted = item.RoomType.isDeleted,
                    Id = item.RoomType.Id,
                    RoleName = item.RoomType.RoleName,
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
}