using System.Collections;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using Microsoft.AspNetCore.JsonPatch;

namespace HealthCare.Domain.Services;

public class PatientService : IPatientService {
    private IPatientRepository _patientRepository;
    private ICredentialsRepository _credentialsRepository;
    private IMedicalRecordRepository _medicalRecordRepository;

    public PatientService(IPatientRepository patientRepository, ICredentialsRepository credentialsRepository, IMedicalRecordRepository medicalRecordRepository) {
        _patientRepository = patientRepository;
        _credentialsRepository = credentialsRepository;
        _medicalRecordRepository = medicalRecordRepository;
    }

    private PatientDomainModel parseToModel(Patient item) {
        PatientDomainModel patientModel = new PatientDomainModel {
            IsDeleted = item.IsDeleted,
            BirthDate = item.BirthDate,
            BlockedBy = item.BlockedBy,
            BlockingCounter = item.BlockingCounter,
            Email = item.Email,
            Id = item.Id,
            Name = item.Name,
            Surname = item.Surname,
            Phone = item.Phone
        };
        if (item.Credentials != null) {
            patientModel.Credentials = new CredentialsDomainModel {
                Id = item.Credentials.Id,
                Username = item.Credentials.Username,
                Password = item.Credentials.Password,
                DoctorId = item.Credentials.DoctorId,
                SecretaryId = item.Credentials.SecretaryId,
                ManagerId = item.Credentials.ManagerId,
                PatientId = item.Credentials.PatientId,
                UserRoleId = item.Credentials.UserRoleId

            };
            if (item.Credentials.UserRole != null) {
                patientModel.Credentials.UserRole = new UserRoleDomainModel {
                    Id = item.Credentials.UserRole.Id,
                    RoleName = item.Credentials.UserRole.RoleName,
                    IsDeleted = item.Credentials.UserRole.IsDeleted
                };
            }
        }
        if (item.MedicalRecord != null) {
            patientModel.MedicalRecord = new MedicalRecordDomainModel {
                IsDeleted = item.MedicalRecord.IsDeleted,
                Allergies = item.MedicalRecord.Allergies,
                BedriddenDiseases = item.MedicalRecord.BedriddenDiseases,
                Height = item.MedicalRecord.Height,
                PatientId = item.MedicalRecord.PatientId,
                Weight = item.MedicalRecord.Weight
            };
        }
        patientModel.Examinations = new List<ExaminationDomainModel>();
        patientModel.Operations = new List<OperationDomainModel>();
        if (item.Examinations != null) {
            foreach (Examination examination in item.Examinations) {
                ExaminationDomainModel examinationDomainModel = new ExaminationDomainModel {
                    DoctorId = examination.DoctorId,
                    RoomId = examination.RoomId,
                    PatientId = examination.PatientId,
                    StartTime = examination.StartTime,
                    IsDeleted = examination.IsDeleted
                };
                if (examination.Anamnesis != null) {
                    AnamnesisDomainModel anamnesisDomainModel = new AnamnesisDomainModel {
                        Id = examination.Anamnesis.Id,
                        Description = examination.Anamnesis.Description,
                        ExaminationId = examination.Anamnesis.ExaminationId,
                        IsDeleted = examination.Anamnesis.IsDeleted
                    };
                    examinationDomainModel.Anamnesis = anamnesisDomainModel;
                }
                patientModel.Examinations.Add(examinationDomainModel);
            }
        }
        if (item.Operations != null) {
            foreach (Operation operation in item.Operations) {
                OperationDomainModel operationDomainModel = new OperationDomainModel {
                    DoctorId = operation.DoctorId,
                    RoomId = operation.RoomId,
                    PatientId = operation.PatientId,
                    Duration = operation.Duration,
                    IsDeleted = operation.IsDeleted
                };
                patientModel.Operations.Add(operationDomainModel);
            }
        }
        return patientModel;
    }

    private Patient parseFromModel(PatientDomainModel item) {
        Patient patient = new Patient {
            IsDeleted = item.IsDeleted,
            BirthDate = item.BirthDate,
            BlockedBy = item.BlockedBy,
            BlockingCounter = item.BlockingCounter,
            Email = item.Email,
            Id = item.Id,
            Name = item.Name,
            Surname = item.Surname,
            Phone = item.Phone
        };
        if (item.Credentials != null) {
            patient.Credentials = new Credentials {
                Id = item.Credentials.Id,
                Username = item.Credentials.Username,
                Password = item.Credentials.Password,
                DoctorId = item.Credentials.DoctorId,
                SecretaryId = item.Credentials.SecretaryId,
                ManagerId = item.Credentials.ManagerId,
                PatientId = item.Credentials.PatientId,
                UserRoleId = item.Credentials.UserRoleId

            };
            if (item.Credentials.UserRole != null) {
                patient.Credentials.UserRole = new UserRole {
                    Id = item.Credentials.UserRole.Id,
                    RoleName = item.Credentials.UserRole.RoleName,
                    IsDeleted = item.Credentials.UserRole.IsDeleted
                };
            }
        }
        if (item.MedicalRecord != null) {
            patient.MedicalRecord = new MedicalRecord {
                IsDeleted = item.MedicalRecord.IsDeleted,
                Allergies = item.MedicalRecord.Allergies,
                BedriddenDiseases = item.MedicalRecord.BedriddenDiseases,
                Height = item.MedicalRecord.Height,
                PatientId = item.MedicalRecord.PatientId,
                Weight = item.MedicalRecord.Weight
            };
        }
        patient.Examinations = new List<Examination>();
        patient.Operations = new List<Operation>();
        if (item.Examinations != null) {
            foreach (ExaminationDomainModel examinationModel in item.Examinations) {
                Examination examination = new Examination {
                    DoctorId = examinationModel.DoctorId,
                    RoomId = examinationModel.RoomId,
                    PatientId = examinationModel.PatientId,
                    StartTime = examinationModel.StartTime,
                    IsDeleted = examinationModel.IsDeleted
                };
                if (examinationModel.Anamnesis != null) {
                    Anamnesis anamnesis = new Anamnesis {
                        Id = examinationModel.Anamnesis.Id,
                        Description = examinationModel.Anamnesis.Description,
                        ExaminationId = examinationModel.Anamnesis.ExaminationId,
                        IsDeleted = examinationModel.Anamnesis.IsDeleted
                    };
                    examination.Anamnesis = anamnesis;
                }
                patient.Examinations.Add(examination);
            }
        }
        if (item.Operations != null) {
            foreach (OperationDomainModel operationModel in item.Operations) {
                Operation operation = new Operation {
                    DoctorId = operationModel.DoctorId,
                    RoomId = operationModel.RoomId,
                    PatientId = operationModel.PatientId,
                    Duration = operationModel.Duration,
                    IsDeleted = operationModel.IsDeleted
                };
                patient.Operations.Add(operation);
            }
        }
        return patient;
    }



    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<PatientDomainModel>> GetAll()
    {
        IEnumerable<Patient> data = await _patientRepository.GetAll();
        if (data == null)
            return null;

        List<PatientDomainModel> results = new List<PatientDomainModel>();
        PatientDomainModel patientModel;
        foreach (Patient item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<PatientDomainModel>> ReadAll()
    {
        IEnumerable<PatientDomainModel> patients = await GetAll();
        List<PatientDomainModel> result = new List<PatientDomainModel>();
        foreach (PatientDomainModel patientModel in patients)
        {
            if (!patientModel.IsDeleted) result.Add(patientModel);
        }
        return result;
    }

    public async Task<PatientDomainModel> Block(decimal patientId)
    {
        Patient patient = await _patientRepository.GetPatientById(patientId);
        // TODO: Fix this with a cookie in the future
        patient.BlockedBy = "Secretary";
        patient.BlockingCounter++;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();
        return parseToModel(patient);
    }

    public async Task<PatientDomainModel> Unblock(decimal patientId)
    {
        Patient patient = await _patientRepository.GetPatientById(patientId);
        // TODO: Fix this with a cookie in the future
        patient.BlockedBy = "";
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();

        return parseToModel(patient);
    }

    public async Task<PatientDomainModel> Add(PatientDomainModel patientModel)
    {
        Patient newPatient = new Patient();
        newPatient.BlockedBy = null;
        newPatient.IsDeleted = false;
        newPatient.Name = patientModel.Name;
        newPatient.Surname = patientModel.Surname;
        newPatient.BlockingCounter = 0;
        newPatient.Email = patientModel.Email;
        newPatient.BirthDate = patientModel.BirthDate;
        newPatient.Phone = patientModel.Phone;
        newPatient.Id = 0;

        Patient insertedPatient = _patientRepository.Post(newPatient);
        _patientRepository.Save();

        MedicalRecord medicalRecord = new MedicalRecord();
        medicalRecord.Height = patientModel.MedicalRecord.Height;
        medicalRecord.Weight = patientModel.MedicalRecord.Weight;
        medicalRecord.BedriddenDiseases = patientModel.MedicalRecord.BedriddenDiseases;
        medicalRecord.Allergies = patientModel.MedicalRecord.Allergies;
        medicalRecord.IsDeleted = false;
        medicalRecord.PatientId = insertedPatient.Id;

        _ = _medicalRecordRepository.Post(medicalRecord);
        _medicalRecordRepository.Save();

        Credentials newCredentials = new Credentials();
        newCredentials.Username = patientModel.Credentials.Username;
        newCredentials.Password = patientModel.Credentials.Password;
        newCredentials.DoctorId = null;
        newCredentials.SecretaryId = null;
        newCredentials.ManagerId = null;
        newCredentials.PatientId = insertedPatient.Id;
        newCredentials.UserRoleId = 726243269;
        newCredentials.isDeleted = false;
        newCredentials.Id = 0;

        _ = _credentialsRepository.Post(newCredentials);
        _credentialsRepository.Save();

        return patientModel;
    }

    public async Task<PatientDomainModel> Update(PatientDomainModel patientModel)
    {

        Patient patient = await _patientRepository.GetPatientById(patientModel.Id);
        patient.Name = patientModel.Name;
        patient.Surname = patientModel.Surname;
        patient.Email = patientModel.Email;
        patient.BirthDate = patientModel.BirthDate;
        patient.Phone = patientModel.Phone;
        patient.BlockedBy = patientModel.BlockedBy;
        patient.BlockingCounter = patientModel.BlockingCounter;
        patient.IsDeleted = patientModel.IsDeleted;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();

        MedicalRecord medicalRecord = await _medicalRecordRepository.GetByPatientId(patient.Id);
        medicalRecord.Height = patientModel.MedicalRecord.Height;
        medicalRecord.Weight = patientModel.MedicalRecord.Weight;
        medicalRecord.BedriddenDiseases = patientModel.MedicalRecord.BedriddenDiseases;
        medicalRecord.Allergies = patientModel.MedicalRecord.Allergies;
        medicalRecord.IsDeleted = patientModel.MedicalRecord.IsDeleted;
        _ = _medicalRecordRepository.Update(medicalRecord);
        _medicalRecordRepository.Save();


        Credentials credentials = await _credentialsRepository.GetCredentialsByPatientId(patient.Id);
        credentials.Username = patientModel.Credentials.Username;
        credentials.Password = patientModel.Credentials.Password;
        _ = _credentialsRepository.Update(credentials);
        _credentialsRepository.Save();
        return parseToModel(patient);
    }


    public async Task<PatientDomainModel> Delete(decimal id)
    {
        Patient patient = await _patientRepository.GetPatientById(id);
        patient.IsDeleted = true;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();
        return new PatientDomainModel();
    }

    public async Task<IEnumerable<PatientDomainModel>> GetBlockedPatients()
    {
        IEnumerable<PatientDomainModel> patients = await GetAll();
        List<PatientDomainModel> blockedPatients = new List<PatientDomainModel>();
        foreach (PatientDomainModel patientModel in patients)
        {
            if (patientModel.BlockedBy != null && !patientModel.BlockedBy.Equals(""))
            {
                blockedPatients.Add(patientModel);
            }
        }

        return blockedPatients;
    }


    public async Task<PatientDomainModel> GetWithMedicalRecord(decimal id)
    {
        Patient patient = await _patientRepository.GetWithMedicalRecord(id);

        if (patient != null)
        {
            PatientDomainModel patientModel = new PatientDomainModel
            {
                IsDeleted = patient.IsDeleted,
                BirthDate = patient.BirthDate,
                BlockedBy = patient.BlockedBy,
                BlockingCounter = patient.BlockingCounter,
                Email = patient.Email,
                Id = patient.Id,
                Name = patient.Name,
                Surname = patient.Surname,
                Phone = patient.Phone
            };


            if (patient.MedicalRecord != null)
            {
                patientModel.MedicalRecord = new MedicalRecordDomainModel
                {
                    IsDeleted = patient.MedicalRecord.IsDeleted,
                    Allergies = patient.MedicalRecord.Allergies,
                    BedriddenDiseases = patient.MedicalRecord.BedriddenDiseases,
                    Height = patient.MedicalRecord.Height,
                    PatientId = patient.MedicalRecord.PatientId,
                    Weight = patient.MedicalRecord.Weight
                };
            }

            return patientModel;
        } 
        return null; 
    }
}