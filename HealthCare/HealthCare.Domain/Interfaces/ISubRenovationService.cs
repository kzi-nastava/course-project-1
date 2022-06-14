using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface ISubRenovationService<CreateDTO, DomainModel>
        where DomainModel : RenovationDomainModel
        where CreateDTO : CreateRenovationDTO
    {
        public Task<DomainModel> Create(CreateDTO newRenovation);
        public Task<IEnumerable<DomainModel>> GetAll();
    }
}
