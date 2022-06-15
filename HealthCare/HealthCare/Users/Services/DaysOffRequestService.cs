using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
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
        private IDaysOffRequestRepository _daysOffRequestRepository;
        private IDoctorRepository _doctorRepository;

        private IAvailabilityService _availabilityService;

        public DaysOffRequestService(IDaysOffRequestRepository daysOffRequestRepository, IDoctorRepository doctorRepository,
                                     IAvailabilityService availabilityService)
        {
            _daysOffRequestRepository = daysOffRequestRepository;
            _doctorRepository = doctorRepository;

            _availabilityService = availabilityService;
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

        public async Task<IEnumerable<DaysOffRequestDomainModel>> GetAllForDoctor(decimal id)
        {
            IEnumerable<DaysOffRequest> data = await _daysOffRequestRepository.GetAllByDoctorId(id);
            if (data == null)
                return new List<DaysOffRequestDomainModel>();

            List<DaysOffRequestDomainModel> results = new List<DaysOffRequestDomainModel>();
            foreach (DaysOffRequest item in data)
            {
                results.Add(ParseToModel(item));
            }

            return results;
        }

        public async Task<DaysOffRequestDomainModel> Create(CreateDaysOffRequestDTO daysOffRequestDTO)
        {
            await validateRequestData(daysOffRequestDTO);

            DaysOffRequest daysOffRequest = _daysOffRequestRepository.Post(ParseFromDTO(daysOffRequestDTO));
            _daysOffRequestRepository.Save();

            return ParseToModel(daysOffRequest);
        }

        public async Task<Boolean> Approve(decimal id)
        {
            DaysOffRequest daysOff = await _daysOffRequestRepository.GetById(id);
            if (!daysOff.State.Equals("created")) throw new DaysOffRequestAlreadyHandledException();
            daysOff.State = "approved";
            _ = _daysOffRequestRepository.Update(daysOff);
            _daysOffRequestRepository.Save();
            return true;
        }

        public async Task<Boolean> Reject(RejectDaysOffRequestDTO dto)
        {
            DaysOffRequest daysOff = await _daysOffRequestRepository.GetById(dto.Id);
            if (!daysOff.State.Equals("created")) throw new DaysOffRequestAlreadyHandledException();
            daysOff.State = "rejected";
            daysOff.RejectionReason = dto.Comment;
            _ = _daysOffRequestRepository.Update(daysOff);
            _daysOffRequestRepository.Save();
            return true;
        }

        private async Task validateRequestData(CreateDaysOffRequestDTO daysOffRequestDTO)
        {
            if (!daysOffRequestDTO.IsUrgent)
            {
                checkIfItsTooLate(daysOffRequestDTO);
                await _availabilityService.IsDoctorFreeOnDateRange(daysOffRequestDTO.From, daysOffRequestDTO.To, daysOffRequestDTO.DoctorId);
            }
            else
            {
                if ((daysOffRequestDTO.To - daysOffRequestDTO.From).Days > 5)
                    throw new NumberOfUrgentDaysOfNotAllowedException();
            }
        }

        private void checkIfItsTooLate(CreateDaysOffRequestDTO daysOffRequestDTO)
        {
            if ((daysOffRequestDTO.From - DateTime.Now).Days < 2)
                throw new LateForDaysOffRequestException();
        }

        private DaysOffRequest ParseFromDTO(CreateDaysOffRequestDTO daysOffRequest)
        {
            return new DaysOffRequest
            {
                DoctorId = daysOffRequest.DoctorId,
                Comment = daysOffRequest.Comment,
                From = daysOffRequest.From,
                To = daysOffRequest.To,
                IsUrgent = daysOffRequest.IsUrgent,
                State = daysOffRequest.IsUrgent ? "approved" : "created"
            };
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
                IsUrgent = daysOffRequest.IsUrgent,
                From = daysOffRequest.From,
                To = daysOffRequest.To
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
