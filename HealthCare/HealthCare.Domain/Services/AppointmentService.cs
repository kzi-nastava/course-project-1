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
    public class AppointmentService : IAppointmentService
    {
        private IExaminationRepository _examinationRepository;
        private IOperationRepository _operationRepository;

        public AppointmentService(IExaminationRepository examinationRepository, IOperationRepository operationRepoaitory)
        {
            _examinationRepository = examinationRepository;
            _operationRepository = operationRepoaitory;
        }

        public async Task<IEnumerable<AppointmentDomainModel>> GetAll()
        {
            return new List<AppointmentDomainModel>();
        }

        public async Task<IEnumerable<AppointmentDomainModel>> GetAllForDoctor(DoctorsScheduleDTO dto)
        {
            IEnumerable<Examination> examinationData = await _examinationRepository.GetAllByDoctorId(dto.DoctorId, dto.Date);
            List<AppointmentDomainModel> results = new List<AppointmentDomainModel>();
            foreach (Examination item in examinationData)
            {
                results.Add(parseToModel(item));
            }

            IEnumerable<Operation> operationData = await _operationRepository.GetAllByDoctorId(dto.DoctorId, dto.Date);
            foreach (Operation item in operationData)
            {
                results.Add(parseToModel(item));
            }

            return results;
        }

        private AppointmentDomainModel parseToModel(Examination examination)
        {
            AppointmentDomainModel appointmentModel = new AppointmentDomainModel
            {
                Id = examination.Id,
                StartTime = examination.StartTime,
                Duration = 15,
                DoctorId = examination.DoctorId,
                IsDeleted = examination.IsDeleted,
                PatientId = examination.PatientId,
                RoomId = examination.RoomId,
                Type = Appointment.Examination
            };
            if (examination.Anamnesis != null)
            {
                appointmentModel.Anamnesis = new AnamnesisDomainModel
                {
                    Id = examination.Anamnesis.Id,
                    Description = examination.Anamnesis.Description,
                    ExaminationId = examination.Anamnesis.ExaminationId,
                    IsDeleted = examination.Anamnesis.IsDeleted
                };
            }
            return appointmentModel;
        }

        private AppointmentDomainModel parseToModel(Operation operation)
        {
            AppointmentDomainModel appointmentModel = new AppointmentDomainModel
            {
                Id = operation.Id,
                StartTime = operation.StartTime,
                Duration = operation.Duration,
                DoctorId = operation.DoctorId,
                IsDeleted = operation.IsDeleted,
                PatientId = operation.PatientId,
                RoomId = operation.RoomId,
                Anamnesis = null,
                Type = Appointment.Operation
            };

            return appointmentModel;
        }
    }
}
