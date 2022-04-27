using HealthCare.Data.Entities;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface ITransferService : IService<TransferDomainModel> {
    public Task<TransferDomainModel> Add(TransferDomainModel newTransfer);
    public Task<IEnumerable<TransferDomainModel>> DoTransfers();

}