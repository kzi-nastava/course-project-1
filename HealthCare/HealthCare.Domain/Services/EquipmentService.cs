using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentService : IEquipmentService
{
    private IEquipmentRepository _equipmentRepository;
    private IInventoryRepository _inventoryRepository;
    private IRoomRepository _roomRepository;
    public EquipmentService(IEquipmentRepository equipmentRepository, 
                            IInventoryRepository inventoryRepository, 
                            IRoomRepository roomRepository) 
    {
        _equipmentRepository = equipmentRepository;
        _inventoryRepository = inventoryRepository;
        _roomRepository = roomRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<EquipmentDomainModel>> ReadAll()
    {
        IEnumerable<EquipmentDomainModel> equipment = await GetAll();
        List<EquipmentDomainModel> result = new List<EquipmentDomainModel>();
        foreach (EquipmentDomainModel item in equipment)
        {
            if (!item.IsDeleted) result.Add(item);
        }

        return result;
    }
    public async Task<IEnumerable<EquipmentDomainModel>> GetAll()
    {
        IEnumerable<Equipment> equipment = await _equipmentRepository.GetAll();
        if (equipment == null)
            return new List<EquipmentDomainModel>();

        List<EquipmentDomainModel> results = new List<EquipmentDomainModel>();
        foreach (Equipment item in equipment)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }
    public async Task<IEnumerable<EquipmentDomainModel>> SearchByName(string substring)
    {
        substring = substring.ToLower();
        IEnumerable<Equipment> equipment = await _equipmentRepository.GetAll();
        if (equipment == null)
            throw new DataIsNullException();

        List<EquipmentDomainModel> results = new List<EquipmentDomainModel>();

        foreach (Equipment item in equipment)
        {
            // added equipment type to search review
            if(item.Name.ToLower().Contains(substring) || item.EquipmentType.Name.ToLower().Contains(substring))
                results.Add(parseToModel(item));
        }
        return results;
    }

    public static EquipmentDomainModel parseToModel(Equipment equipment)
    {
        EquipmentDomainModel equipmentModel = new EquipmentDomainModel 
        {
            Id = equipment.Id,
            EquipmentTypeId = equipment.equipmentTypeId,
            Name = equipment.Name,
            IsDeleted = equipment.IsDeleted
        };
        
        if (equipment.EquipmentType != null)
            equipmentModel.EquipmentType = EquipmentTypeService.parseToModel(equipment.EquipmentType);
        
        return equipmentModel;
    }
    
    public static Equipment parseFromModel(EquipmentDomainModel equipmentModel)
    {
        Equipment equipment = new Equipment 
        {
            Id = equipmentModel.Id,
            equipmentTypeId = equipmentModel.EquipmentTypeId,
            Name = equipmentModel.Name,
            IsDeleted = equipmentModel.IsDeleted
        };
        
        if (equipmentModel.EquipmentType != null)
            equipment.EquipmentType = EquipmentTypeService.parseFromModel(equipmentModel.EquipmentType);
        
        return equipment;
    }

    private IEnumerable<EquipmentDomainModel> parseToModels(IEnumerable<Equipment> equipments)
    {
        List<EquipmentDomainModel> results = new List<EquipmentDomainModel>();
        foreach (Equipment equipment in equipments)
        {
            results.Add(parseToModel(equipment));
        }
        return results;
    }

    public async Task<IEnumerable<EquipmentDomainModel>> Filter(FilterEquipmentDTO dto)
    {
        IEnumerable<Equipment> filterResult = await _equipmentRepository.GetAll();
        if (filterResult == null || filterResult.Count() < 1)
            throw new DataIsNullException();

        // filter #1
        if (dto.EquipmentTypeId != null)
        {
            filterResult = filterResult.Where(e => e.equipmentTypeId == dto.EquipmentTypeId);
        }


        if (dto.MinAmount != null || dto.MaxAmount != null)
        {
            IEnumerable<Inventory> inventories = await _inventoryRepository.GetAll();
            // group inventories by equipment and sum the amount
            var summedEquipment = inventories.GroupBy(inventory => inventory.EquipmentId)
                .Select(group => new
                {
                    EquipmentId = group.Key,
                    TotalAmount = group.Sum(i => i.Amount),
                });
            //filter #2
            if (dto.MinAmount != null)
            {
                IEnumerable<decimal> minFilteredEquipmentIds = summedEquipment.Where(group => group.TotalAmount > dto.MinAmount)
                    .Select(group => group.EquipmentId);

                filterResult = filterResult.Where(x => minFilteredEquipmentIds.Contains(x.Id));
            }

            // filter #3
            if (dto.MaxAmount != null)
            {
                IEnumerable<decimal> maxFilteredEquipmentIds = summedEquipment.Where(group => group.TotalAmount < dto.MaxAmount)
                    .Select(group => group.EquipmentId);

                filterResult = filterResult.Where(x => maxFilteredEquipmentIds.Contains(x.Id));
            }
        }

        // filter #4
        if(dto.RoomTypeId != null)
        {
            // get rooms ids of that room type
            IEnumerable<Room> rooms = await _roomRepository.GetAll();
            IEnumerable<decimal> roomIds = rooms.Where(x => x.RoomTypeId == dto.RoomTypeId).Select(r => r.Id);

            // find equipment ids in all inventories stored in the rooms
            IEnumerable<Inventory> inventories = await _inventoryRepository.GetAll();
            IEnumerable<decimal> equipmentIds = inventories.Where(i => roomIds.Contains(i.RoomId)).Select(x => x.EquipmentId); 

            // return equipment with those ids
            filterResult = filterResult.Where(x => equipmentIds.Contains(x.Id));

        }
        return parseToModels(filterResult);
    }

}