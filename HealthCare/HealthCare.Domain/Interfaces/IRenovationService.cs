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
        public Task<RenovationDomainModel> Create(RenovationDomainModel newRenovation);
    }
}
