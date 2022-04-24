using System.Collections;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForUpdate;
using HealthCare.Repositories;
using Microsoft.AspNetCore.JsonPatch;

namespace HealthCare.Domain.Services;

public class PatientService : IPatientService{
    private IPatientRepository _patientRepository;
    private ICredentialsRepository _credentialsRepository;
    private IMedicalRecordRepository _medicalRecordRepository;

    public PatientService(IPatientRepository patientRepository, ICredentialsRepository credentialsRepository, IMedicalRecordRepository medicalRecordRepository) {
        _patientRepository = patientRepository;
        _credentialsRepository = credentialsRepository;
        _medicalRecordRepository = medicalRecordRepository;
    }

    private PatientDomainModel parseToDomainModel(Patient item) {
        PatientDomainModel patientModel = new PatientDomainModel {
            isDeleted = item.isDeleted,
            BirthDate = item.BirthDate,
            blockedBy = item.blockedBy,
            blockingCounter = item.blockingCounter,
            //Credentials = item.Credentials,
            Email = item.Email,
            //Examinations = item.Examinations,
            Id = item.Id,
            //MedicalRecord = item.MedicalRecord,
            Name = item.Name,
            Surname = item.Surname,
            //Operations = item.Operations,
            Phone = item.Phone
        };
        if (item.Credentials != null) {
            patientModel.Credentials = new CredentialsDomainModel {
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
                patientModel.Credentials.UserRole = new UserRoleDomainModel {
                    Id = item.Credentials.UserRole.Id,
                    RoleName = item.Credentials.UserRole.RoleName,
                    isDeleted = item.Credentials.UserRole.isDeleted
                };
            }
        }
        if (item.MedicalRecord != null) {
            patientModel.MedicalRecord = new MedicalRecordDomainModel {
                isDeleted = item.MedicalRecord.isDeleted,
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
            foreach (var examination in item.Examinations) {
                ExaminationDomainModel examinationDomainModel = new ExaminationDomainModel {
                    doctorId = examination.doctorId,
                    roomId = examination.roomId,
                    patientId = examination.patientId,
                    StartTime = examination.StartTime,
                    IsDeleted = examination.IsDeleted
                };
                if (examination.Anamnesis != null) {
                    AnamnesisDomainModel anamnesisDomainModel = new AnamnesisDomainModel {
                        Description = examination.Anamnesis.Description,
                        doctorId = examination.Anamnesis.doctorId,
                        roomId = examination.Anamnesis.roomId,
                        patientId = examination.Anamnesis.patientId,
                        StartTime = examination.Anamnesis.StartTime,
                        isDeleted = examination.Anamnesis.isDeleted
                    };
                examinationDomainModel.Anamnesis = anamnesisDomainModel;
                patientModel.Examinations.Add(examinationDomainModel);
                }
            }
        }
        if (item.Operations != null) {
            foreach (var operation in item.Operations) {
                OperationDomainModel operationDomainModel = new OperationDomainModel {
                    DoctorId = operation.DoctorId,
                    RoomId = operation.DoctorId,
                    PatientId = operation.DoctorId,
                    Duration = operation.Duration,
                    isDeleted = operation.isDeleted
                };
                patientModel.Operations.Add(operationDomainModel);
            }
        }
        return patientModel;
    }

    private Patient parseToModel(PatientDomainModel item) {
        Patient patientModel = new Patient {
            isDeleted = item.isDeleted,
            BirthDate = item.BirthDate,
            blockedBy = item.blockedBy,
            blockingCounter = item.blockingCounter,
            //Credentials = item.Credentials,
            Email = item.Email,
            //Examinations = item.Examinations,
            Id = item.Id,
            //MedicalRecord = item.MedicalRecord,
            Name = item.Name,
            Surname = item.Surname,
            //Operations = item.Operations,
            Phone = item.Phone
        };
        if (item.Credentials != null) {
            patientModel.Credentials = new Credentials {
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
                patientModel.Credentials.UserRole = new UserRole {
                    Id = item.Credentials.UserRole.Id,
                    RoleName = item.Credentials.UserRole.RoleName,
                    isDeleted = item.Credentials.UserRole.isDeleted
                };
            }
        }
        if (item.MedicalRecord != null) {
            patientModel.MedicalRecord = new MedicalRecord {
                isDeleted = item.MedicalRecord.isDeleted,
                Allergies = item.MedicalRecord.Allergies,
                BedriddenDiseases = item.MedicalRecord.BedriddenDiseases,
                Height = item.MedicalRecord.Height,
                PatientId = item.MedicalRecord.PatientId,
                Weight = item.MedicalRecord.Weight
            };
        }
        patientModel.Examinations = new List<Examination>();
        patientModel.Operations = new List<Operation>();
        if (item.Examinations != null) {
            foreach (var examination in item.Examinations) {
                Examination examinationDomainModel = new Examination {
                    doctorId = examination.doctorId,
                    roomId = examination.roomId,
                    patientId = examination.patientId,
                    StartTime = examination.StartTime,
                    IsDeleted = examination.IsDeleted
                };
                if(examination.Anamnesis != null) {
                    Anamnesis anamnesisDomainModel = new Anamnesis {
                        Description = examination.Anamnesis.Description,
                        doctorId = examination.Anamnesis.doctorId,
                        roomId = examination.Anamnesis.roomId,
                        patientId = examination.Anamnesis.patientId,
                        StartTime = examination.Anamnesis.StartTime,
                        isDeleted = examination.Anamnesis.isDeleted
                    };
                    examinationDomainModel.Anamnesis = anamnesisDomainModel;
                }
                patientModel.Examinations.Add(examinationDomainModel);
            }
        }
        if (item.Operations != null) {
            foreach (var operation in item.Operations) {
                Operation operationDomainModel = new Operation{
                    DoctorId = operation.DoctorId,
                    RoomId = operation.DoctorId,
                    PatientId = operation.DoctorId,
                    Duration = operation.Duration,
                    isDeleted = operation.isDeleted
                };
                patientModel.Operations.Add(operationDomainModel);
            }
        }
        return patientModel;
    }



    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<PatientDomainModel>> GetAll()
    {
        var data = await _patientRepository.GetAll();
        if (data == null)
            return null;

        List<PatientDomainModel> results = new List<PatientDomainModel>();
        PatientDomainModel patientModel;
        foreach (var item in data)
        {
            patientModel = parseToDomainModel(item);
            results.Add(patientModel);
        }

        return results;
    }

    public async Task<PatientDomainModel> Block(decimal patientId)
    {
        Patient patient = await _patientRepository.GetPatientById(patientId);
        // TODO: Fix this with a cookie in the future
        patient.blockedBy = "Secretary";
        patient.blockingCounter++;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();
        return parseToDomainModel(patient);
    }
    
    public async Task<PatientDomainModel> Unblock(decimal patientId)
    {
        Patient patient = await _patientRepository.GetPatientById(patientId);
        // TODO: Fix this with a cookie in the future
        patient.blockedBy = "";
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();
        
        return parseToDomainModel(patient);
    }

    public async Task<CreatePatientDomainModel> Add(CreatePatientDomainModel patientModel)
    {
        Patient newPatient = new Patient();
        newPatient.blockedBy = null;
        newPatient.isDeleted = false;
        newPatient.Name = patientModel.Name;
        newPatient.Surname = patientModel.Surname;
        newPatient.blockingCounter = 0;
        newPatient.Email = patientModel.Email;
        newPatient.BirthDate = patientModel.BirthDate;
        newPatient.Phone = patientModel.Phone;
        newPatient.Id = 0;
        
        Patient insertedPatient =  _patientRepository.Post(newPatient);
        _patientRepository.Save();
        
        MedicalRecord medicalRecord = new MedicalRecord();
        medicalRecord.Height = patientModel.MedicalRecord.Height;
        medicalRecord.Weight = patientModel.MedicalRecord.Weight;
        medicalRecord.BedriddenDiseases = patientModel.MedicalRecord.BedriddenDiseases;
        medicalRecord.Allergies = patientModel.MedicalRecord.Allergies;
        medicalRecord.isDeleted = false;
        medicalRecord.PatientId = insertedPatient.Id;

        _ = _medicalRecordRepository.Post(medicalRecord);
        _medicalRecordRepository.Save();
        
        Credentials newCredentials = new Credentials();
        newCredentials.Username = patientModel.Credentials.Username;
        newCredentials.Password = patientModel.Credentials.Password;
        newCredentials.doctorId = null;
        newCredentials.secretaryId = null;
        newCredentials.managerId = null;
        newCredentials.patientId = insertedPatient.Id;
        newCredentials.userRoleId = 726243269;
        newCredentials.isDeleted = false;
        newCredentials.Id = 0;
        
        _ = _credentialsRepository.Post(newCredentials);
        _credentialsRepository.Save();

        return patientModel;
    }

    public async Task<PatientDomainModel> Update(UpdatePatientDomainModel patientModel, decimal id)
    {
        
        Patient patient = await _patientRepository.GetPatientById(id);
        patient.Name = patientModel.Name;
        patient.Surname = patientModel.Surname;
        patient.Email = patientModel.Email;
        patient.BirthDate = patientModel.BirthDate;
        patient.Phone = patientModel.Phone;
        patient.blockedBy = patientModel.blockedBy;
        patient.blockingCounter = patientModel.blockingCounter;
        patient.isDeleted = patientModel.isDeleted;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();

        MedicalRecord medicalRecord = await _medicalRecordRepository.GetByPatientId(id);
        medicalRecord.Height = patientModel.MedicalRecord.Height;
        medicalRecord.Weight = patientModel.MedicalRecord.Weight;
        medicalRecord.BedriddenDiseases = patientModel.MedicalRecord.BedriddenDiseases;
        medicalRecord.Allergies = patientModel.MedicalRecord.Allergies;
        medicalRecord.isDeleted = patientModel.MedicalRecord.isDeleted;
        _ = _medicalRecordRepository.Update(medicalRecord);
        _medicalRecordRepository.Save();


        Credentials credentials = await _credentialsRepository.GetCredentialsByPatientId(id);
        credentials.Username = patientModel.Credentials.Username;
        credentials.Password = patientModel.Credentials.Password;
        _ = _credentialsRepository.Update(credentials);
        _credentialsRepository.Save();
        return null;
    }
   

    public async Task<PatientDomainModel> Delete(decimal id)
    {
        Patient patient = await _patientRepository.GetPatientById(id);
        patient.isDeleted = true;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();
        return null;   
    }

    public async Task<IEnumerable<PatientDomainModel>> GetBlockedPatients()
    {
        IEnumerable<PatientDomainModel> patients = await GetAll();
        List<PatientDomainModel> blockedPatients = new List<PatientDomainModel>();
        foreach (var patient in patients)
        {
            if (patient.blockedBy != null && !patient.blockedBy.Equals(""))
            {
                blockedPatients.Add(patient);
            }
        }

        return blockedPatients;
    }

}