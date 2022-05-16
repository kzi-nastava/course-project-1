using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentRequestService : IEquipmentRequestService
{
    private readonly IEquipmentRequestRepository _equipmentRequestRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IRoomRepository _roomRepository;

    public EquipmentRequestService(IEquipmentRequestRepository equipmentRequestRepository,
        IEquipmentRepository equipmentRepository,
        IInventoryRepository inventoryRepository,
        IRoomRepository roomRepository)
    {
        _equipmentRequestRepository = equipmentRequestRepository;
        _equipmentRepository = equipmentRepository;
        _inventoryRepository = inventoryRepository;
        _roomRepository = roomRepository;
    }

    public async Task<IEnumerable<EquipmentRequestDomainModel>> GetAll()
    {
        IEnumerable<EquipmentRequest> equipmentRequests = await _equipmentRequestRepository.GetAll();
        if (equipmentRequests == null)
            throw new DataIsNullException();

        List<EquipmentRequestDomainModel> results = new List<EquipmentRequestDomainModel>();
        foreach (EquipmentRequest item in equipmentRequests)
            results.Add(ParseToModel(item));

        return results;
    }

    public async Task<Dictionary<decimal, EquipmentDomainModel>> MakeMissingEquipment()
    {
        Dictionary<decimal, EquipmentDomainModel> result = new Dictionary<decimal, EquipmentDomainModel>();
        List<Equipment> equipment = (List<Equipment>) await _equipmentRepository.GetAll();
        foreach (Equipment item in equipment)
        {
            EquipmentDomainModel equipmentModel = EquipmentService.ParseToModel(item);
            if (!result.ContainsKey(equipmentModel.Id)) result.Add(equipmentModel.Id, equipmentModel);
        }
        return result;
    }

    public async Task<List<EquipmentDomainModel>> FilterMissingEquipment(Dictionary<decimal, EquipmentDomainModel> missingEquipment)
    {
        List<EquipmentDomainModel> result = new List<EquipmentDomainModel>();
        List<Inventory> inventories = (List<Inventory>)await _inventoryRepository.GetAll();
        foreach (Inventory item in inventories)
        {
            InventoryDomainModel inventoryModel = InventoryService.ParseToModel(item);
            if (missingEquipment.ContainsKey(inventoryModel.EquipmentId) && item.Amount != 0) missingEquipment.Remove(inventoryModel.EquipmentId);
        }
        foreach (KeyValuePair<decimal, EquipmentDomainModel> pair in missingEquipment)
            result.Add(pair.Value);
        return result;
    }

    public async Task<IEnumerable<EquipmentDomainModel>> GetMissingEquipment()
    {
        Dictionary<decimal, EquipmentDomainModel> missingEquipment = await MakeMissingEquipment();
        return await FilterMissingEquipment(missingEquipment);
    }

    public async Task<IEnumerable<EquipmentRequestDomainModel>> OrderEquipment(IEnumerable<EquipmentRequestDTO> dtos)
    {
        List<EquipmentRequestDomainModel> result = new List<EquipmentRequestDomainModel>();
        foreach (EquipmentRequestDTO dto in dtos)
            result.Add(MakeEquipmentRequest(dto));
        _equipmentRequestRepository.Save();
        return result;
    }

    public EquipmentRequestDomainModel MakeEquipmentRequest(EquipmentRequestDTO dto)
    {
        EquipmentRequestDomainModel equipmentRequestModel = GetModelFromDto(dto);
        _ = _equipmentRequestRepository.Post(ParseFromModel(equipmentRequestModel));
        return equipmentRequestModel;
    }

    public EquipmentRequestDomainModel GetModelFromDto(EquipmentRequestDTO dto)
    {
        EquipmentRequestDomainModel equipmentRequestModel = new EquipmentRequestDomainModel
        {
            ExecutionTime = removeSeconds(DateTime.Now.AddDays(1)),
            IsExecuted = false,
            Amount = dto.Amount,
            EquipmentId = dto.EquipmentId
        };
        
        return equipmentRequestModel;
    }
    
    private static DateTime removeSeconds(DateTime dateTime)
    {
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;
        int hour = dateTime.Hour;
        int minute = dateTime.Minute;
        int second = 0;
        return new DateTime(year, month, day, hour, minute, second);
    }


    public async Task<IEnumerable<EquipmentRequestDomainModel>> DoAllOrders()
    {
        Room storage = await _roomRepository.GetRoomByName("storage");
        List<EquipmentRequest> equipmentRequest = 
            (List<EquipmentRequest>) await _equipmentRequestRepository.GetAll();
        List<EquipmentRequestDomainModel> result = new List<EquipmentRequestDomainModel>();
        foreach (var item in equipmentRequest)
        {
            if (item.IsExecuted) continue;
            ParseRequest(item, storage);
            result.Add(ParseToModel(item));
            item.IsExecuted = true;
            _equipmentRequestRepository.Update(item);
        }
        _ = _roomRepository.Update(storage);
        _roomRepository.Save();
        _equipmentRepository.Save();
        return result;
    }

    public void ParseRequest(EquipmentRequest equipmentRequest, Room storage)
    {
        foreach (Inventory item in storage.Inventories)
        {
            if (item.EquipmentId == equipmentRequest.EquipmentId)
            {
                item.Amount += equipmentRequest.Amount;
                return;
            }
        }

        InventoryDomainModel inventoryModel = GetInventoryModel(equipmentRequest, storage.Id);
        storage.Inventories.Add(InventoryService.ParseFromModel(inventoryModel));
    }

    public InventoryDomainModel GetInventoryModel(EquipmentRequest equipmentRequest, decimal roomId)
    {
        InventoryDomainModel inventoryModel = new InventoryDomainModel
        {
            Amount = equipmentRequest.Amount,
            EquipmentId = equipmentRequest.EquipmentId,
            IsDeleted = false,
            RoomId = roomId
        };
        
        return inventoryModel;
    }

    public static EquipmentRequestDomainModel ParseToModel(EquipmentRequest equipmentRequest)
    {
        EquipmentRequestDomainModel equipmentRequestModel = new EquipmentRequestDomainModel 
        {
            Id = equipmentRequest.Id,
            EquipmentId = equipmentRequest.EquipmentId,
            Amount = equipmentRequest.Amount,
            ExecutionTime = equipmentRequest.ExecutionTime,
            IsExecuted = equipmentRequest.IsExecuted
        };
        
        if (equipmentRequest.Equipment != null)
            equipmentRequestModel.Equipment = EquipmentService.ParseToModel(equipmentRequest.Equipment);
        
        return equipmentRequestModel;
    }
    
    public static EquipmentRequest ParseFromModel(EquipmentRequestDomainModel equipmentRequestModel)
    {
        EquipmentRequest equipmentRequest = new EquipmentRequest 
        {
            Id = equipmentRequestModel.Id,
            EquipmentId = equipmentRequestModel.EquipmentId,
            Amount = equipmentRequestModel.Amount,
            ExecutionTime = equipmentRequestModel.ExecutionTime,
            IsExecuted = equipmentRequestModel.IsExecuted
        };
        
        if (equipmentRequestModel.Equipment != null)
            equipmentRequest.Equipment = EquipmentService.ParseFromModel(equipmentRequestModel.Equipment);
        
        return equipmentRequest;
    }
}
