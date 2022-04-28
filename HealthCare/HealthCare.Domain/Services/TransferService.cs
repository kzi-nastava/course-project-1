using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class TransferService : ITransferService{
    private ITransferRepository _transferRepository;
    private IInventoryRepository _inventoryRepository;
    private IEquipmentRepository _equipmentRepository;

    public TransferService(ITransferRepository transferRepository, IEquipmentRepository equipmentRepository, IInventoryRepository inventioryRepository) {
        _transferRepository = transferRepository;
        _inventoryRepository = inventioryRepository;
        _equipmentRepository = equipmentRepository; 

    }

    public async Task<IEnumerable<TransferDomainModel>> GetAll()
    {
        IEnumerable<Transfer> transfers = await _transferRepository.GetAll();
        if (transfers == null)
            return null;

        List<TransferDomainModel> results = new List<TransferDomainModel>();
        TransferDomainModel transferModel;
        foreach (var item in transfers)
        {
            transferModel = new TransferDomainModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                Amount = item.Amount,
                EquipmentId = item.EquipmentId,
                //RoomFrom = item.RoomFrom,
                RoomIdOut = item.RoomIdOut,
                //RoomTo = item.RoomTo,
                RoomIdIn = item.RoomIdIn,
                TransferTime = item.TransferTime,
                Executed = item.Executed
            };
            if (item.Equipment != null) {
                transferModel.Equipment = new EquipmentDomainModel {
                    Id = item.Equipment.Id,
                    EquipmentTypeId = item.Equipment.equipmentTypeId,
                    IsDeleted = item.Equipment.IsDeleted,
                    Name = item.Equipment.Name
                };
                if (item.Equipment.EquipmentType != null)
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

    public async Task<TransferDomainModel> Add(TransferDomainModel newTransfer)
    {
        Inventory inRoomInventory = await _inventoryRepository.GetInventoryById(newTransfer.RoomIdIn, newTransfer.EquipmentId);

        Inventory outRoomInventory = await _inventoryRepository.GetInventoryById(newTransfer.RoomIdOut, newTransfer.EquipmentId);
        _inventoryRepository.Update(outRoomInventory);

        if (inRoomInventory == null)
        {
            inRoomInventory = new Inventory
            {
                Amount = newTransfer.Amount,
                RquipmentId = newTransfer.EquipmentId,
                RoomId = newTransfer.RoomIdIn,
                IsDeleted = false
            };               
            _inventoryRepository.Post(inRoomInventory);
        }
        else
        {      
            _inventoryRepository.Update(inRoomInventory);
        }


        Transfer transfer = await _transferRepository.GetTransferById(newTransfer.Id);
        if(transfer == null)
        {
            transfer = new Transfer
            { 
                RoomIdIn = newTransfer.RoomIdIn,
                RoomIdOut = newTransfer.RoomIdOut,
                TransferTime = newTransfer.TransferTime,
                Amount = newTransfer.Amount,
                EquipmentId = newTransfer.EquipmentId,
                Executed = newTransfer.Executed,
            };
        }
        
        _transferRepository.Post(transfer);
        _inventoryRepository.Save();
        return parseToModel(transfer);
    }

    private TransferDomainModel parseToModel(Transfer transfer) => new TransferDomainModel
    {
        Id = transfer.Id,
        RoomIdOut= transfer.RoomIdOut,
        RoomIdIn= transfer.RoomIdIn,
        TransferTime= transfer.TransferTime,
        Amount= transfer.Amount,
        EquipmentId = transfer.EquipmentId,
        Executed = transfer.Executed,
    };

    public async Task<IEnumerable<TransferDomainModel>> DoTransfers()
    {
        IEnumerable<Transfer> transfers = await _transferRepository.GetAll();
        if (transfers == null)
            return new List<TransferDomainModel>();

        List<TransferDomainModel> transfersExecuted = new List<TransferDomainModel>();

        foreach(var transfer in transfers)
        {
            if(transfer.TransferTime < DateTime.UtcNow)
            {
                var roomIn = await _inventoryRepository.GetInventoryById(transfer.RoomIdIn, transfer.EquipmentId);
                var roomOut = await _inventoryRepository.GetInventoryById(transfer.RoomIdOut, transfer.EquipmentId);
                roomIn.Amount += transfer.Amount;
                roomOut.Amount -= transfer.Amount;
                transfer.Executed = true;
                transfersExecuted.Add(parseToModel(transfer));
            }
        }
        _transferRepository.Save();
        return transfersExecuted;
    }
     
    public async Task<IEnumerable<TransferDomainModel>> ReadAll()
    {
        IEnumerable<TransferDomainModel> transfers = await GetAll();
        List<TransferDomainModel> result = new List<TransferDomainModel>();
        foreach (var item in transfers)
        {
            if(!item.IsDeleted) result.Add(item);
        }
        return result;
    } 
}