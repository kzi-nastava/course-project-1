using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IMedicalRecordService : IService<MedicalRecordDomainModel> {
    public Task<MedicalRecordDomainModel> GetForPatient(decimal id);
    public Task<MedicalRecordDomainModel> Update(MedicalRecordDomainModel model);
    public Task<IEnumerable<MedicalRecordDomainModel>> ReadAll();
}