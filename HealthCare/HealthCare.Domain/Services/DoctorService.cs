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

    private Doctor parseFromModel(DoctorDomainModel doctorModel)
    {
        Doctor doctor = new Doctor 
        {
            IsDeleted = doctorModel.IsDeleted,
            BirthDate = doctorModel.BirthDate,
            //Credentials = item.Credentials,
            Email = doctorModel.Email,
            Id = doctorModel.Id,
            Name = doctorModel.Name,
            Phone = doctorModel.Phone,
            Surname = doctorModel.Surname,
            SpecializationId = doctorModel.SpecializationId
        };
        if (doctorModel.Credentials != null)
        {
            doctor.Credentials = new Credentials
            {
                Id = doctorModel.Credentials.Id,
                Username = doctorModel.Credentials.Username,
                Password = doctorModel.Credentials.Password,
                DoctorId = doctorModel.Credentials.DoctorId,
                SecretaryId = doctorModel.Credentials.SecretaryId,
                ManagerId = doctorModel.Credentials.ManagerId,
                PatientId = doctorModel.Credentials.PatientId,
                UserRoleId = doctorModel.Credentials.UserRoleId

            };
            if (doctorModel.Credentials.UserRole != null)
            {
                doctor.Credentials.UserRole = new UserRole
                {
                    Id = doctorModel.Credentials.UserRole.Id,
                    RoleName = doctorModel.Credentials.UserRole.RoleName,
                    IsDeleted = doctorModel.Credentials.UserRole.IsDeleted
                };
            }
        }
        if(doctorModel.Specialization != null)
        {
            doctor.Specialization = new Specialization
            {
                Id = doctorModel.Specialization.Id,
                Name = doctorModel.Specialization.Name
            };

        }
        doctor.Examinations = new List<Examination>();
        doctor.Operations = new List<Operation>();
        if (doctorModel.Examinations != null) 
        {
            foreach (ExaminationDomainModel examinationModel in doctorModel.Examinations) 
            {
                Examination examination = new Examination 
                {
                    DoctorId = examinationModel.DoctorId,
                    RoomId = examinationModel.RoomId,
                    PatientId = examinationModel.PatientId,
                    StartTime = examinationModel.StartTime,
                    IsDeleted = examinationModel.IsDeleted
                };
                if (examinationModel.Anamnesis != null)
                {
                    Anamnesis anamnesis = new Anamnesis
                    {
                        Id = examinationModel.Anamnesis.Id,
                        Description = examinationModel.Anamnesis.Description,
                        ExaminationId = examinationModel.Anamnesis.ExaminationId,
                        IsDeleted = examinationModel.Anamnesis.IsDeleted

                    };
                examination.Anamnesis = anamnesis;
                }
                doctor.Examinations.Add(examination);

            }
        }
        if(doctorModel.Operations != null) 
        {
            foreach (OperationDomainModel operationModel in doctorModel.Operations) 
            {
                Operation operation = new Operation 
                {
                    DoctorId = operationModel.DoctorId,
                    RoomId = operationModel.RoomId,
                    PatientId = operationModel.PatientId,
                    Duration = operationModel.Duration,
                    IsDeleted = operationModel.IsDeleted,
                    IsEmergency = operationModel.IsEmergency
                };
                doctor.Operations.Add(operation);
            }
        }
        return doctor;
    }
    private DoctorDomainModel parseToModel(Doctor doctor)
    {
        DoctorDomainModel doctorModel = new DoctorDomainModel 
        {
            IsDeleted = doctor.IsDeleted,
            BirthDate = doctor.BirthDate,
            //Credentials = item.Credentials,
            Email = doctor.Email,
            Id = doctor.Id,
            Name = doctor.Name,
            Phone = doctor.Phone,
            Surname = doctor.Surname,
            SpecializationId = doctor.SpecializationId
        };
        if (doctor.Credentials != null)
        {
            doctorModel.Credentials = new CredentialsDomainModel
            {
                Id = doctor.Credentials.Id,
                Username = doctor.Credentials.Username,
                Password = doctor.Credentials.Password,
                DoctorId = doctor.Credentials.DoctorId,
                SecretaryId = doctor.Credentials.SecretaryId,
                ManagerId = doctor.Credentials.ManagerId,
                PatientId = doctor.Credentials.PatientId,
                UserRoleId = doctor.Credentials.UserRoleId

            };
            if (doctor.Credentials.UserRole != null)
            {
                doctorModel.Credentials.UserRole = new UserRoleDomainModel
                {
                    Id = doctor.Credentials.UserRole.Id,
                    RoleName = doctor.Credentials.UserRole.RoleName,
                    IsDeleted = doctor.Credentials.UserRole.IsDeleted
                };
            }
        }
        if(doctor.Specialization != null)
        {
            doctorModel.Specialization = new SpecializationDomainModel
            {
                Id = doctor.Specialization.Id,
                Name = doctor.Specialization.Name
            };

        }
        doctorModel.Examinations = new List<ExaminationDomainModel>();
        doctorModel.Operations = new List<OperationDomainModel>();
        if (doctor.Examinations != null) 
        {
            foreach (Examination examination in doctor.Examinations) 
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
        if(doctor.Operations != null) 
        {
            foreach (Operation operation in doctor.Operations) 
            {
                OperationDomainModel operationModel = new OperationDomainModel 
                {
                    DoctorId = operation.DoctorId,
                    RoomId = operation.RoomId,
                    PatientId = operation.PatientId,
                    Duration = operation.Duration,
                    IsDeleted = operation.IsDeleted,
                    IsEmergency = operation.IsEmergency
                };
                doctorModel.Operations.Add(operationModel);
            }
        }
        return doctorModel;
    }
    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<DoctorDomainModel>> GetAll()
    {
        IEnumerable<Doctor> data = await _doctorRepository.GetAll();
        if (data == null)
            return new List<DoctorDomainModel>();

        List<DoctorDomainModel> results = new List<DoctorDomainModel>();
        foreach (Doctor item in data) 
        {
            results.Add(parseToModel(item));
        }
        return results;
    }


    public async Task<IEnumerable<DoctorDomainModel>> GetAllBySpecialization(decimal id)
    {
        IEnumerable<Doctor> data = await _doctorRepository.GetBySpecialization(id);
        if (data == null)
            return new List<DoctorDomainModel>();

        List<DoctorDomainModel> results = new List<DoctorDomainModel>();
        foreach (Doctor item in data)
        {
            results.Add(parseToModel(item));
        }
        return results;
    }

    public async Task<DoctorDomainModel> GetById(decimal id)
    {
        Doctor data = await _doctorRepository.GetDoctorById(id);
        if (data == null)
            return null;
        return parseToModel(data);
    }

    public async Task<IEnumerable<DoctorDomainModel>> ReadAll()
    {
        IEnumerable<DoctorDomainModel> doctors = await GetAll();
        List<DoctorDomainModel> result = new List<DoctorDomainModel>();
        foreach (DoctorDomainModel doctor in doctors)
        {
            if (!doctor.IsDeleted) result.Add(doctor);
        }
        return result;
    }
    
    private DateTime removeSeconds(DateTime dateTime)
    {
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;
        int hour = dateTime.Hour;
        int minute = dateTime.Minute;
        int second = 0;
        return new DateTime(year, month, day, hour, minute, second);
    }

    public async Task<IEnumerable<KeyValuePair<DateTime, DateTime>>> GetAvailableSchedule(decimal doctorId, decimal duration=15)
    {
        Doctor doctor = await _doctorRepository.GetDoctorById(doctorId);
        DoctorDomainModel doctorModel = parseToModel(doctor);
        List<KeyValuePair<DateTime, DateTime>> schedule = new List<KeyValuePair<DateTime, DateTime>>();
        DateTime timeStart, timeEnd;
        // Go through examinations
        foreach (ExaminationDomainModel item in  doctorModel.Examinations)
        {
            if (item.IsDeleted) continue;
            timeStart = removeSeconds(item.StartTime);
            timeEnd = removeSeconds(item.StartTime).AddMinutes(15);
            schedule.Add(new KeyValuePair<DateTime, DateTime>(timeStart, timeEnd));
        }
        // Go through operations
        foreach (OperationDomainModel item in  doctorModel.Operations)
        {
            if (item.IsDeleted) continue;
            timeStart = removeSeconds(item.StartTime);
            timeEnd = removeSeconds(item.StartTime).AddMinutes((double) item.Duration);
            schedule.Add(new KeyValuePair<DateTime, DateTime>(timeStart, timeEnd));
        }
        // Sort the list
        schedule.Sort((x, y) => x.Key.CompareTo(y.Key));
        // Generate available time
        List<KeyValuePair<DateTime, DateTime>> result = new List<KeyValuePair<DateTime, DateTime>>();
        if (schedule.Count == 0) return result;
        KeyValuePair<DateTime, DateTime> first = schedule[0];
        for (int i = 1; i < schedule.Count; i++)
        {
            var currentFirst = first.Value.AddMinutes((double)duration);
            var currentSecond = schedule[i].Key;
            if (currentFirst <= currentSecond)  result.Add(new KeyValuePair<DateTime, DateTime>(first.Value, currentSecond.AddMinutes((double) -duration)));
            first = schedule[i];
        }
        result.Add(new KeyValuePair<DateTime, DateTime>(schedule[schedule.Count - 1].Value, new DateTime(9999, 12, 31)));
        return result;
    }
}