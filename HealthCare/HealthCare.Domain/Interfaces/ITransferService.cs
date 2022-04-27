using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface ITransferService : IService<TransferDomainModel>
{
    public Task<IEnumerable<TransferDomainModel>> ReadAll();

}