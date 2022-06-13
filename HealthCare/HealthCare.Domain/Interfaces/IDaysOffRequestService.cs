using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IDaysOffRequestService : IService<DaysOffRequestDomainModel>
    {
        public Task<DaysOffRequestDomainModel> Create(CreateDaysOffRequestDTO daysOffRequest);
    }
}
