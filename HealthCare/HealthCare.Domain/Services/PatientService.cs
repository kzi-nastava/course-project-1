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
}