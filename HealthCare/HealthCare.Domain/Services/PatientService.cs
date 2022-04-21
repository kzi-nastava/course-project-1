using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class PatientService : IPatientService{
    private IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository) {
        _patientRepository = patientRepository;
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
            patientModel = new PatientDomainModel
            {
                isDeleted = item.isDeleted,
                BirthDate = item.BirthDate,
                blockedBy = item.blockedBy,
                blockingCounter = item.blockingCounter,
                Credentials = item.Credentials,
                Email = item.Email,
                Examinations = item.Examinations,
                Id = item.Id,
                MedicalRecord = item.MedicalRecord,
                Name = item.Name,
                Surname = item.Surname,
                Operations = item.Operations,
                Phone = item.Phone
            };
            results.Add(patientModel);
        }

        return results;
    }

    public async Task<PatientDomainModel> Add(PatientDomainModel patientModel)
    {
        Patient patient = new Patient();
        patient.blockedBy = patientModel.blockedBy;
        patient.isDeleted = patientModel.isDeleted;
        patient.Name = patientModel.Name;
        patient.Surname = patientModel.Surname;
        patient.blockingCounter = patientModel.blockingCounter;
        patient.Credentials = patientModel.Credentials;
        patient.Email = patientModel.Email;
        patient.Examinations = patientModel.Examinations;
        patient.Id = patientModel.Id;
        patient.Operations = patientModel.Operations;
        patient.MedicalRecord = patientModel.MedicalRecord;
        patient.BirthDate = patientModel.BirthDate;
        patient.Phone = patientModel.Phone;

        Patient insertedPatient = _patientRepository.Post(patient);
        _patientRepository.Save();

        return patientModel;
    }

    public async Task<PatientDomainModel> Update(PatientDomainModel patientModel)
    {
        Patient patient = await _patientRepository.GetPatientById(patientModel.Id);

        patient.blockedBy = patientModel.blockedBy;
        patient.isDeleted = patientModel.isDeleted;
        patient.Name = patientModel.Name;
        patient.Surname = patientModel.Surname;
        patient.blockingCounter = patientModel.blockingCounter;
        patient.Credentials = patientModel.Credentials;
        patient.Email = patientModel.Email;
        patient.Examinations = patientModel.Examinations;
        patient.Id = patientModel.Id;
        patient.Operations = patientModel.Operations;
        patient.MedicalRecord = patientModel.MedicalRecord;
        patient.BirthDate = patientModel.BirthDate;
        patient.Phone = patientModel.Phone;

        Patient insertedPatient = _patientRepository.Update(patient);
        _patientRepository.Save();

        return patientModel;
    }

    public async Task<PatientDomainModel> Delete(PatientDomainModel patientModel)
    {
        Patient patient = await _patientRepository.GetPatientById(patientModel.Id);

        patient.blockedBy = patientModel.blockedBy;
        patient.isDeleted = patientModel.isDeleted;
        patient.Name = patientModel.Name;
        patient.Surname = patientModel.Surname;
        patient.blockingCounter = patientModel.blockingCounter;
        patient.Credentials = patientModel.Credentials;
        patient.Email = patientModel.Email;
        patient.Examinations = patientModel.Examinations;
        patient.Id = patientModel.Id;
        patient.Operations = patientModel.Operations;
        patient.MedicalRecord = patientModel.MedicalRecord;
        patient.BirthDate = patientModel.BirthDate;
        patient.Phone = patientModel.Phone;

        Patient deletedPatient = _patientRepository.Delete(patient);
        _patientRepository.Save();

        return patientModel;
        
    }
}