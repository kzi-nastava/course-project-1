using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class DaysOffRequestService : IDaysOffRequestService
    {
        IDaysOffRequestRepository _daysOffRequestRepository;

        public DaysOffRequestService(IDaysOffRequestRepository daysOffRequestRepository)
        {
            _daysOffRequestRepository = daysOffRequestRepository;
        }

        public async Task<IEnumerable<DaysOffRequestDomainModel>> GetAll()
        {
            IEnumerable<DaysOffRequest> data = await _daysOffRequestRepository.GetAll();
            if (data == null)
                return new List<DaysOffRequestDomainModel>();

            List<DaysOffRequestDomainModel> results = new List<DaysOffRequestDomainModel>();
            foreach (DaysOffRequest item in data)
            {
                results.Add(ParseToModel(item));
            }

            return results;
        }

        private DaysOffRequestDomainModel ParseToModel(DaysOffRequest daysOffRequest)
        {
            return new DaysOffRequestDomainModel
            {
                Id = daysOffRequest.Id,
                State = TranslateState(daysOffRequest.State),
                Comment = daysOffRequest.Comment,
                RejectionReason = daysOffRequest.RejectionReason,
                DoctorId = daysOffRequest.DoctorId,
                IsUrgent = daysOffRequest.IsUrgent
            };
        }

        private DaysOffRequestState TranslateState(string state)
        {
            switch (state)
            {
                case "created": return DaysOffRequestState.CREATED; break;
                case "approved": return DaysOffRequestState.APPROVED; break;
                case "rejected": return DaysOffRequestState.REJECTED; break;
                default: throw new Exception("Undefined days off request state");
            }
        }
    }
}
