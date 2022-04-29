using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IMedicalRecordService : IService<MedicalRecordDomainModel> 
{
    public Task<MedicalRecordDomainModel> GetForPatient(decimal id);
    public Task<MedicalRecordDomainModel> Update(MedicalRecordDomainModel medicalRecordModel);
    public Task<IEnumerable<MedicalRecordDomainModel>> ReadAll();
}