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
    public class AppointmentService : IAppointmentService
    {
        private IExaminationRepository _examinationRepository;
        private IOperationRepository _operationRepository;

        public AppointmentService(IExaminationRepository examinationRepository, IOperationRepository operationRepoaitory)
        {
            _examinationRepository = examinationRepository;
            _operationRepository = operationRepoaitory;
        }

        public Task<IEnumerable<AppointmentDomainModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppointmentDomainModel>> GetAllForDoctor(decimal id, DateTime date)
        {
            var examinationData = await _examinationRepository.GetAllByDoctorId(id, date);

            List<AppointmentDomainModel> results = new List<AppointmentDomainModel>();

            foreach (var item in examinationData)
            {
                results.Add(parseToModel(item));
            }

            var operationData = await _operationRepository.GetAllByDoctorId(id, date);

            foreach (var item in operationData)
            {
                results.Add(parseToModel(item));
            }

            return results;
        }

        private AppointmentDomainModel parseToModel(Examination examination)
        {
            AppointmentDomainModel appointmentModel = new AppointmentDomainModel
            {
                StartTime = examination.StartTime,
                Duration = 15,
                doctorId = examination.doctorId,
                IsDeleted = examination.IsDeleted,
                patientId = examination.patientId,
                roomId = examination.roomId,
                type = Appointment.Examination
            };
            if (examination.Anamnesis != null)
            {
                appointmentModel.Anamnesis = new AnamnesisDomainModel
                {
                    Id = examination.Anamnesis.Id,
                    Description = examination.Anamnesis.Description,
                    ExaminationId = examination.Anamnesis.ExaminationId,
                    isDeleted = examination.Anamnesis.isDeleted
                };
            }
            return appointmentModel;
        }

        private AppointmentDomainModel parseToModel(Operation operation)
        {
            AppointmentDomainModel appointmentModel = new AppointmentDomainModel
            {
                StartTime = operation.StartTime,
                Duration = operation.Duration,
                doctorId = operation.DoctorId,
                IsDeleted = operation.isDeleted,
                patientId = operation.PatientId,
                roomId = operation.RoomId,
                Anamnesis = null,
                type = Appointment.Operation
            };

            return appointmentModel;
        }


    }
}
