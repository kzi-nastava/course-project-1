using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentService : IEquipmentService{
    private IEquipmentRepository _equipmentRepository;
    private IInventoryRepository _inventoryRepository;
    private IRoomRepository _roomRepository;
    public EquipmentService(IEquipmentRepository equipmentRepository, IInventoryRepository inventoryRepository, IRoomRepository roomRepository) {
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
        foreach (var item in equipment)
        {
            if (!item.IsDeleted) result.Add(item);
        }

        return result;
    }
    public async Task<IEnumerable<EquipmentDomainModel>> GetAll()
    {
        var data = await _equipmentRepository.GetAll();
        if (data == null)
            return null;

        List<EquipmentDomainModel> results = new List<EquipmentDomainModel>();
        EquipmentDomainModel equipmentModel;
        foreach (var item in data)
        {
            equipmentModel = new EquipmentDomainModel
            {
                Id = item.Id,
                equipmentTypeId = item.equipmentTypeId,
                IsDeleted = item.IsDeleted,
                Name = item.Name,
            };
            if (item.EquipmentType != null)
                equipmentModel.EquipmentType = new EquipmentTypeDomainModel {
                    Id = item.EquipmentType.Id,
                    Name = item.EquipmentType.Name,
                    IsDeleted = item.EquipmentType.IsDeleted
                };
            results.Add(equipmentModel);
        }

        return results;
    }
    public async Task<IEnumerable<EquipmentDomainModel>> SearchByName(string nameAlike)
    {
        nameAlike = nameAlike.ToLower();
        var data = await _equipmentRepository.GetAll();
        if (data == null)
            return null;

        List<EquipmentDomainModel> results = new List<EquipmentDomainModel>();

        foreach (var item in data)
        {
            Console.WriteLine(item.EquipmentType.Name);
            // added equipment type to search review
            if(item.Name.ToLower().Contains(nameAlike) || item.EquipmentType.Name.ToLower().Contains(nameAlike))
                results.Add(parseToModel(item));
        }

        return results;
    }

    private EquipmentDomainModel parseToModel(Equipment item) => new EquipmentDomainModel
    {
        Id = item.Id,
        //EquipmentType = item.EquipmentType,
        equipmentTypeId = item.equipmentTypeId,
        Name = item.Name,
        IsDeleted = item.IsDeleted,

    };

    private IEnumerable<EquipmentDomainModel> parseToModel(IEnumerable<Equipment> input)
    {
        var results = new List<EquipmentDomainModel>();
        foreach (var item in input)
        {
            results.Add(parseToModel((Equipment)item));
        }
        return results;
    }

    public async Task<IEnumerable<EquipmentDomainModel>> Filter(decimal equipmentTypeId, int minAmmount, int maxAmmount, decimal roomTypeId)
    {
        var result = await _equipmentRepository.GetAll();
        if (result == null || result.Count() < 1)
            return null;

        // filter #1
        if (equipmentTypeId != -1)
        {
            result = result.Where(e => e.equipmentTypeId == equipmentTypeId);
        }
            

        if(minAmmount != -1 || maxAmmount != -1)
        {
            var inventories = await _inventoryRepository.GetAll();
            // group inventories by equipment and sum the ammount
            var summedEquipment = inventories.GroupBy(inventory => inventory.equipmentId)
                .Select(group => new
                {
                    EquipmentId = group.Key,
                    TotalAmount = group.Sum(i => i.Amount),
                });
            //filter #2
            if (minAmmount != -1)
            {
                var minFilteredEquipmentIds = summedEquipment.Where(group => group.TotalAmount > minAmmount)
                    .Select(group => group.EquipmentId);

                result = result.Where(x => minFilteredEquipmentIds.Contains(x.Id));
            }

            // filter #3
            if (maxAmmount != -1)
            {
                var maxFilteredEquipmentIds = summedEquipment.Where(group => group.TotalAmount < maxAmmount)
                    .Select(group => group.EquipmentId);

                result = result.Where(x => maxFilteredEquipmentIds.Contains(x.Id));
            }
        }

        // filter #4
        if(roomTypeId != -1)
        {
            // get rooms ids of that room type
            var rooms = await _roomRepository.GetAll();
            var roomIds = rooms.Where(x => x.RoomTypeId == roomTypeId).Select(r => r.Id);

            // find equipment ids in all inventories stored in the rooms
            var inventories = await _inventoryRepository.GetAll();
            var equipmentIds = inventories.Where(i => roomIds.Contains(i.roomId)).Select(x => x.equipmentId); 

            // return equipment with those ids
            var equipment = await _equipmentRepository.GetAll();
            result = result.Where(x => equipmentIds.Contains(x.Id));

        }
        return parseToModel(result);
    }


    public  async Task<Tuple<EquipmentDomainModel, EquipmentDomainModel>> Transfer(decimal roomIdIn, decimal roomIdOut, decimal equipmentID, decimal amount)
    {
        return null;
    }





    //maybe yes maybe no
    public interface IFilter<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> input);
    }

    public abstract class Pipeline<T>
    {
        protected List<IFilter<T>> filters = new List<IFilter<T>>();
        public Pipeline<T> Register(IFilter<T> filter)
        {
            filters.Add(filter);
            return this;
        }
        public abstract IEnumerable<T> Process(IEnumerable<T> input);
    }

    public class EquipmentSelectionPipeline : Pipeline<Equipment>
    {
        public override IEnumerable<Equipment> Process(IEnumerable<Equipment> input)
        {
            foreach(var filter in filters)
            {
                input = filter.Execute(input);
            }
            return input;
        }
    }

    public class EquipmentTypeFilter : IFilter<Equipment>
    {
        private EquipmentType equipmentType;
        EquipmentTypeFilter(EquipmentType equipmentType)
        {
            this.equipmentType = equipmentType;
        }
        public IEnumerable<Equipment> Execute(IEnumerable<Equipment> input)
        {
            IEnumerable<Equipment> filteredEquipment;
            if (input == null || input.Count() == 0)
            {
                return input;
            }
            filteredEquipment = input.Where(equipment => equipment.EquipmentType.Equals(this.equipmentType)).ToList();
            return filteredEquipment;
        }
    }


}