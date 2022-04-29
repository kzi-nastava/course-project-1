using HealthCare.Data.Entities;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace HealthCare.Domain.Interfaces;

public interface IPatientService : IService<PatientDomainModel>
{
    public Task<PatientDomainModel> Add(PatientDomainModel patientModel);
    public Task<PatientDomainModel> Update(PatientDomainModel patientModel);
    public Task<PatientDomainModel> Delete(PatientDomainModel patientModel);
    public Task<PatientDomainModel> Block(decimal id);
    public Task<PatientDomainModel> Unblock(decimal id);
    public Task<IEnumerable<PatientDomainModel>> GetBlockedPatients();
    public Task<PatientDomainModel> GetWithMedicalRecord(decimal id);
    public Task<IEnumerable<PatientDomainModel>> ReadAll();
}