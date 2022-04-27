using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class TransferService : ITransferService{
    private ITransferRepository _transferRepository;

    public TransferService(ITransferRepository transferRepository) {
        _transferRepository = transferRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<TransferDomainModel>> GetAll()
    {
        var data = await _transferRepository.GetAll();
        if (data == null)
            return null;

        List<TransferDomainModel> results = new List<TransferDomainModel>();
        TransferDomainModel transferModel;
        foreach (var item in data)
        {
            transferModel = new TransferDomainModel
            {
                isDeleted = item.isDeleted,
                Amount = item.Amount,
                EquipmentId = item.EquipmentId,
                //RoomFrom = item.RoomFrom,
                RoomFromId = item.RoomFromId,
                //RoomTo = item.RoomTo,
                RoomToId = item.RoomToId,
                TransferTime = item.TransferTime
            };
            if (item.Equipment != null) {
                transferModel.Equipment = new EquipmentDomainModel {
                    Id = item.Equipment.Id,
                    equipmentTypeId = item.Equipment.equipmentTypeId,
                    IsDeleted = item.Equipment.IsDeleted,
                    Name = item.Equipment.Name,
                };
                if (transferModel.Equipment.EquipmentType != null)
                    transferModel.Equipment.EquipmentType = new EquipmentTypeDomainModel {
                        Id = item.Equipment.EquipmentType.Id,
                        Name = item.Equipment.EquipmentType.Name,
                        IsDeleted = item.Equipment.EquipmentType.IsDeleted
                    };
            }
            results.Add(transferModel);
        }

        return results;
    } 
    public async Task<IEnumerable<TransferDomainModel>> ReadAll()
    {
        IEnumerable<TransferDomainModel> transfers = await GetAll();
        List<TransferDomainModel> result = new List<TransferDomainModel>();
        foreach (var item in transfers)
        {
            if(!item.isDeleted) result.Add(item);
        }
        return result;
    } 
}