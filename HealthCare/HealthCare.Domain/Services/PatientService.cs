using System.Collections;
using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using Microsoft.AspNetCore.JsonPatch;

namespace HealthCare.Domain.Services;

public class PatientService : IPatientService 
{
    private IPatientRepository _patientRepository;
    private ICredentialsRepository _credentialsRepository;
    private IMedicalRecordRepository _medicalRecordRepository;

    public PatientService(IPatientRepository patientRepository, 
                          ICredentialsRepository credentialsRepository, 
                          IMedicalRecordRepository medicalRecordRepository) 
    {
        _patientRepository = patientRepository;
        _credentialsRepository = credentialsRepository;
        _medicalRecordRepository = medicalRecordRepository;
    }

    private PatientDomainModel parseToModel(Patient patient) 
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
        
        if (patient.Credentials != null)
            patientModel.Credentials = CredentialsService.parseToModel(patient.Credentials);
            
        if (patient.MedicalRecord != null) 
            patientModel.MedicalRecord = MedicalRecordService.parseToModel(patient.MedicalRecord);
        
        patientModel.Examinations = new List<ExaminationDomainModel>();
        patientModel.Operations = new List<OperationDomainModel>();
        if (patient.Examinations != null) 
            foreach (Examination examination in patient.Examinations) 
                patientModel.Examinations.Add(ExaminationService.parseToModel(examination));
        
        if (patient.Operations != null) 
            foreach (Operation operation in patient.Operations) 
                patientModel.Operations.Add(OperationService.parseToModel(operation));
        
        return patientModel;
    }

    private Patient parseFromModel(PatientDomainModel patientModel) 
    {
        Patient patient = new Patient 
        {
            IsDeleted = patientModel.IsDeleted,
            BirthDate = patientModel.BirthDate,
            BlockedBy = patientModel.BlockedBy,
            BlockingCounter = patientModel.BlockingCounter,
            Email = patientModel.Email,
            Id = patientModel.Id,
            Name = patientModel.Name,
            Surname = patientModel.Surname,
            Phone = patientModel.Phone
        };
        if (patientModel.Credentials != null)
            patient.Credentials = CredentialsService.parseFromModel(patientModel.Credentials);

        if (patientModel.MedicalRecord != null)
            patient.MedicalRecord = MedicalRecordService.parseFromModel(patientModel.MedicalRecord);

        patient.Examinations = new List<Examination>();
        patient.Operations = new List<Operation>();
        if (patientModel.Examinations != null) 
            foreach (ExaminationDomainModel examinationModel in patientModel.Examinations) 
                patient.Examinations.Add(ExaminationService.parseFromModel(examinationModel));
            
        if (patientModel.Operations != null) 
            foreach (OperationDomainModel operationModel in patientModel.Operations) 
                patient.Operations.Add(OperationService.parseFromModel(operationModel));
        
        return patient;
    }


    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<PatientDomainModel>> GetAll()
    {
        IEnumerable<Patient> data = await _patientRepository.GetAll();
        if (data == null)
            return new List<PatientDomainModel>();

        List<PatientDomainModel> results = new List<PatientDomainModel>();
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

    public async Task<PatientDomainModel> Create(CUPatientDTO dto)
    {
        Patient newPatient = new Patient
        {
            IsDeleted = false,
            BirthDate = dto.BirthDate,
            Email = dto.Email,
            Name = dto.Name,
            Surname = dto.Surname,
            Phone = dto.Phone
        };


        Patient insertedPatient = _patientRepository.Post(newPatient);
        _patientRepository.Save();

        MedicalRecord medicalRecord = new MedicalRecord();
        medicalRecord.Height = dto.MedicalRecordDTO.Height;
        medicalRecord.Weight = dto.MedicalRecordDTO.Weight;
        medicalRecord.BedriddenDiseases = dto.MedicalRecordDTO.BedriddenDiseases;
        medicalRecord.IsDeleted = false;
        medicalRecord.PatientId = insertedPatient.Id;

        _ = _medicalRecordRepository.Post(medicalRecord);
        _medicalRecordRepository.Save();

        Credentials newCredentials = new Credentials();
        newCredentials.Username = dto.LoginDTO.Username;
        newCredentials.Password = dto.LoginDTO.Password;
        newCredentials.DoctorId = null;
        newCredentials.SecretaryId = null;
        newCredentials.ManagerId = null;
        newCredentials.PatientId = insertedPatient.Id;
        newCredentials.UserRoleId = 726243269;
        newCredentials.isDeleted = false;
        newCredentials.Id = 0;

        _ = _credentialsRepository.Post(newCredentials);
        _credentialsRepository.Save();

        return parseToModel(insertedPatient);
    }

    public async Task<PatientDomainModel> Update(CUPatientDTO dto)
    {

        Patient patient = await _patientRepository.GetPatientById(dto.Id);
        patient.Name = dto.Name;
        patient.Surname = dto.Surname;
        patient.Email = dto.Email;
        patient.BirthDate = dto.BirthDate;
        patient.Phone = dto.Phone;
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();

        MedicalRecord medicalRecord = await _medicalRecordRepository.GetByPatientId(patient.Id);
        medicalRecord.Height = dto.MedicalRecordDTO.Height;
        medicalRecord.Weight = dto.MedicalRecordDTO.Weight;
        medicalRecord.BedriddenDiseases = dto.MedicalRecordDTO.BedriddenDiseases;
        _ = _medicalRecordRepository.Update(medicalRecord);
        _medicalRecordRepository.Save();


        Credentials credentials = await _credentialsRepository.GetCredentialsByPatientId(patient.Id);
        credentials.Username = dto.LoginDTO.Username;
        credentials.Password = dto.LoginDTO.Password;
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
        
        MedicalRecord medicalRecord = await _medicalRecordRepository.GetByPatientId(id);
        medicalRecord.IsDeleted = true;
        _ = _medicalRecordRepository.Update(medicalRecord);
        _medicalRecordRepository.Save();
        Credentials credentials = await _credentialsRepository.GetCredentialsByPatientId(id);
        credentials.isDeleted = true;
        _ = _credentialsRepository.Update(credentials);
        _credentialsRepository.Save();
        
        return parseToModel(patient);
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
        Patient patient = await _patientRepository.GetPatientById(id);
        if (patient == null)
            throw new DataIsNullException();

        return parseToModel(patient);
    }

    public async Task<IEnumerable<KeyValuePair<DateTime, DateTime>>> GetSchedule(decimal id)
    {
        return null;
    }
}