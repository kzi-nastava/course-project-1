using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class AvailabilityService
    {
        private IExaminationRepository _examinationRepository;
        private IOperationRepository _operationRepository;
        private IRoomRepository _roomRepository;
        private IPatientRepository _patientRepository;
        private IDoctorRepository _doctorRepository;

        public AvailabilityService(IExaminationRepository examinationRepository,
                                  IOperationRepository operationRepository,
                                  IRoomRepository roomRepository,
                                  IPatientRepository patientRepository,
                                  IDoctorRepository doctorRepository)
        {
            _examinationRepository = examinationRepository;
            _operationRepository = operationRepository;
            _roomRepository = roomRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        private async Task<bool> isPatientOnExamination(CUExaminationDTO dto)
        {
            IEnumerable<Examination> patientsExaminations = await _examinationRepository.GetAllByPatientId(dto.PatientId);
            foreach (Examination examination in patientsExaminations)
            {
                if (examination.Id != dto.ExaminationId)
                {
                    double difference = (dto.StartTime - examination.StartTime).TotalMinutes;
                    if (difference <= 15 && difference >= -15)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> isPatientOnOperation(CUExaminationDTO dto)
        {
            IEnumerable<Operation> patientsOperations = await _operationRepository.GetAllByPatientId(dto.PatientId);
            foreach (Operation operation in patientsOperations)
            {
                double difference = (dto.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -15)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> isDoctorOnExamination(CUExaminationDTO dto)
        {
            IEnumerable<Examination> doctorsExaminations = await _examinationRepository.GetAllByDoctorId(dto.DoctorId);
            if (doctorsExaminations == null)
            {
                return false;
            }
            foreach (Examination examination in doctorsExaminations)
            {
                if (examination.Id != dto.ExaminationId)
                {
                    double difference = (dto.StartTime - examination.StartTime).TotalMinutes;
                    if (difference <= 15 && difference >= -15)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> isDoctorOnOperation(CUExaminationDTO dto)
        {
            IEnumerable<Operation> doctorsOperations = await _operationRepository.GetAllByDoctorId(dto.DoctorId);
            foreach (Operation operation in doctorsOperations)
            {
                double difference = (dto.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -15)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> isDoctorAvailable(CUExaminationDTO dto)
        {
            return !(await isDoctorOnExamination(dto) ||
                     await isDoctorOnOperation(dto));
        }

        private async Task<bool> isPatientAvailable(CUExaminationDTO dto)
        {
            return !(await isPatientOnExamination(dto) ||
                     await isPatientOnOperation(dto));
        }
        public async Task ValidateUserInput(CUExaminationDTO dto, IPatientService patientService)
        {
            if (dto.StartTime <= DateTime.Now)
                throw new DateInPastExeption();
            if (await patientService.isPatientBlocked(dto.PatientId))
                throw new PatientIsBlockedException();

            bool doctorAvailable = await isDoctorAvailable(dto);
            if (!doctorAvailable)
                throw new DoctorNotAvailableException();

            bool patientAvailable = await isPatientAvailable(dto);
            if (!patientAvailable)
                throw new PatientNotAvailableException();
        }
    }
}
