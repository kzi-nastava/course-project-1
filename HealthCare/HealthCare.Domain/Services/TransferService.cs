using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class TransferService : ITransferService{
    private ITransferRepository _transferRepository;

    public TransferService(ITransferRepository transferRepository) {
        _transferRepository = transferRepository;
    }

    public Task<IEnumerable<TransferDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}