using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces {
    public interface IAntiTrollService : IService<AntiTrollDomainModel> {
        public Task<IEnumerable<AntiTrollDomainModel>> GetByPatientId(decimal patientId);
    }
}
