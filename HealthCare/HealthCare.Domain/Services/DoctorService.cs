using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class DoctorService : IDoctorService
{
    private IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository) 
    {
        _doctorRepository = doctorRepository;
    }
    
    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<DoctorDomainModel>> GetAll()
    {
        IEnumerable<Doctor> data = await _doctorRepository.GetAll();
        if (data == null)
            return new List<DoctorDomainModel>();

        List<DoctorDomainModel> results = new List<DoctorDomainModel>();
        DoctorDomainModel doctorModel;
        foreach (Doctor item in data) 
        {
            doctorModel = new DoctorDomainModel 
            {
                isDeleted = item.IsDeleted,
                BirthDate = item.BirthDate,
                //Credentials = item.Credentials,
                Email = item.Email,
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            if (item.Credentials != null) 
            {
                doctorModel.Credentials = new CredentialsDomainModel 
                {
                    Id = item.Credentials.Id,
                    Username = item.Credentials.Username,
                    Password = item.Credentials.Password,
                    DoctorId = item.Credentials.DoctorId,
                    SecretaryId = item.Credentials.SecretaryId,
                    ManagerId = item.Credentials.ManagerId,
                    PatientId = item.Credentials.PatientId,
                    UserRoleId = item.Credentials.UserRoleId

                };
                if (item.Credentials.UserRole != null) 
                {
                    doctorModel.Credentials.UserRole = new UserRoleDomainModel 
                    {
                        Id = item.Credentials.UserRole.Id,
                        RoleName = item.Credentials.UserRole.RoleName,
                        IsDeleted = item.Credentials.UserRole.IsDeleted
                    };
                }
            }
            doctorModel.Examinations = new List<ExaminationDomainModel>();
            doctorModel.Operations = new List<OperationDomainModel>();
            if (item.Examinations != null) 
            {
                foreach (Examination examination in item.Examinations) 
                {
                    ExaminationDomainModel examinationModel = new ExaminationDomainModel 
                    {
                        DoctorId = examination.DoctorId,
                        RoomId = examination.RoomId,
                        PatientId = examination.PatientId,
                        StartTime = examination.StartTime,
                        IsDeleted = examination.IsDeleted
                    };
                    if (examination.Anamnesis != null)
                    {
                        AnamnesisDomainModel anamnesisModel = new AnamnesisDomainModel
                        {
                            Id = examination.Anamnesis.Id,
                            Description = examination.Anamnesis.Description,
                            ExaminationId = examination.Anamnesis.ExaminationId,
                            IsDeleted = examination.Anamnesis.IsDeleted

                        };
                    examinationModel.Anamnesis = anamnesisModel;
                    }
                    doctorModel.Examinations.Add(examinationModel);

                }
            }
            if(item.Operations != null) 
            {
                foreach (Operation operation in item.Operations) 
                {
                    OperationDomainModel operationModel = new OperationDomainModel 
                    {
                        DoctorId = operation.DoctorId,
                        RoomId = operation.RoomId,
                        PatientId = operation.PatientId,
                        Duration = operation.Duration,
                        IsDeleted = operation.IsDeleted
                    };
                    doctorModel.Operations.Add(operationModel);
                }
                results.Add(doctorModel);
            }
        }
        return results;
    }
    
    public async Task<IEnumerable<DoctorDomainModel>> ReadAll()
    {
        IEnumerable<DoctorDomainModel> doctors = await GetAll();
        List<DoctorDomainModel> result = new List<DoctorDomainModel>();
        foreach (DoctorDomainModel doctor in doctors)
        {
            if (!doctor.isDeleted) result.Add(doctor);
        }
        return result;
    }
}