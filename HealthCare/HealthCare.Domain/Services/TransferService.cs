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
                Equipment = item.Equipment,
                EquipmentId = item.EquipmentId,
                RoomFrom = item.RoomFrom,
                RoomFromId = item.RoomFromId,
                RoomTo = item.RoomTo,
                RoomToId = item.RoomToId,
                TransferTime = item.TransferTime
            };
            results.Add(transferModel);
        }

        return results;
    } 
}