using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Interfaces;

public class MedicalRecordService : IMedicalRecordService {
    private IMedicalRecordRepository _medicalRecordRepository;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository) {
        _medicalRecordRepository = medicalRecordRepository;
    }

    public Task<IEnumerable<MedicalRecordDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}