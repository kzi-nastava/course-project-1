using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IRenovationService : IService<RenovationDomainModel>
    {
        public Task<IEnumerable<RenovationDomainModel>> GetAll();
        public Task<SimpleRenovationDomainModel> Create(SimpleRenovationDomainModel newRenovation);
        public Task<JoinRenovationDomainModel> Create(JoinRenovationDomainModel newRenovation);
        public Task<SplitRenovationDomainModel> Create(SplitRenovationDomainModel newRenovation);
        Task<IEnumerable<JoinRenovationDomainModel>> GetJoin();
        Task<IEnumerable<SplitRenovationDomainModel>> GetSplit();
        Task<IEnumerable<SimpleRenovationDomainModel>> GetSimple();
    }
}
