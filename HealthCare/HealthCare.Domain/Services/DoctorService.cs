using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class DoctorService : IDoctorService{
    private IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository) {
        _doctorRepository = doctorRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<DoctorDomainModel>> GetAll()
    {
        var data = await _doctorRepository.GetAll();
        if (data == null)
            return null;

        List<DoctorDomainModel> results = new List<DoctorDomainModel>();
        DoctorDomainModel doctorModel;
        foreach (var item in data) {
            doctorModel = new DoctorDomainModel {
                isDeleted = item.isDeleted,
                BirthDate = item.BirthDate,
                //Credentials = item.Credentials,
                Email = item.Email,
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            if (item.Credentials != null) {
                doctorModel.Credentials = new CredentialsDomainModel {
                    Id = item.Credentials.Id,
                    Username = item.Credentials.Username,
                    Password = item.Credentials.Password,
                    doctorId = item.Credentials.doctorId,
                    secretaryId = item.Credentials.secretaryId,
                    managerId = item.Credentials.managerId,
                    patientId = item.Credentials.patientId,
                    userRoleId = item.Credentials.userRoleId

                };
                if (item.Credentials.UserRole != null) {
                    doctorModel.Credentials.UserRole = new UserRoleDomainModel {
                        Id = item.Credentials.UserRole.Id,
                        RoleName = item.Credentials.UserRole.RoleName,
                        isDeleted = item.Credentials.UserRole.isDeleted
                    };
                }
            }
            doctorModel.Examinations = new List<ExaminationDomainModel>();
            doctorModel.Operations = new List<OperationDomainModel>();
            if (item.Examinations != null) {
                foreach (var examination in item.Examinations) {
                    ExaminationDomainModel examinationDomainModel = new ExaminationDomainModel {
                        doctorId = examination.doctorId,
                        roomId = examination.roomId,
                        patientId = examination.patientId,
                        StartTime = examination.StartTime,
                        IsDeleted = examination.IsDeleted
                    };
                    AnamnesisDomainModel anamnesisDomainModel = new AnamnesisDomainModel {
                        Description = examination.Anamnesis.Description,
                        doctorId = examination.Anamnesis.doctorId,
                        roomId = examination.Anamnesis.roomId,
                        patientId = examination.Anamnesis.patientId,
                        StartTime = examination.Anamnesis.StartTime,
                        isDeleted = examination.Anamnesis.isDeleted

                    };
                    examinationDomainModel.Anamnesis = anamnesisDomainModel;
                    doctorModel.Examinations.Add(examinationDomainModel);
                }
            }
            if(item.Operations != null) {
                foreach (var operation in item.Operations) {
                    OperationDomainModel operationDomainModel = new OperationDomainModel {
                        DoctorId = operation.DoctorId,
                        RoomId = operation.DoctorId,
                        PatientId = operation.DoctorId,
                        Duration = operation.Duration,
                        isDeleted = operation.isDeleted
                    };
                    doctorModel.Operations.Add(operationDomainModel);
                }
                results.Add(doctorModel);
            }
        }
        

        return results;
    }
}