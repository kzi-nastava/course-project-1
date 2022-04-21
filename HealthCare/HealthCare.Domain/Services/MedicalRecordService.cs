using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Interfaces;

public class MedicalRecordService : IMedicalRecordService {
    private IMedicalRecordRepository _medicalRecordRepository;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository) {
        _medicalRecordRepository = medicalRecordRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<MedicalRecordDomainModel>> GetAll()
    {
        var data = await _medicalRecordRepository.GetAll();
        if (data == null)
            return null;

        List<MedicalRecordDomainModel> results = new List<MedicalRecordDomainModel>();
        MedicalRecordDomainModel medicalRecordModel;
        foreach (var item in data)
        {
            medicalRecordModel = new MedicalRecordDomainModel
            {
                isDeleted = item.isDeleted,
                Allergies = item.Allergies,
                BedriddenDiseases = item.BedriddenDiseases,
                Height = item.Height,
                Patient = item.Patient,
                PatientId = item.PatientId,
                Weight = item.Weight
            };
            results.Add(medicalRecordModel);
        }

        return results;
    } 
}