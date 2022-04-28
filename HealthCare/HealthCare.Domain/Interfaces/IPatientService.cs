using HealthCare.Data.Entities;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForUpdate;
using Microsoft.AspNetCore.JsonPatch;

namespace HealthCare.Domain.Interfaces;

public interface IPatientService : IService<PatientDomainModel>
{
    public Task<CreatePatientDomainModel> Add(CreatePatientDomainModel patientModel);
    public Task<PatientDomainModel> Update(UpdatePatientDomainModel patientModel, decimal id);
    public Task<PatientDomainModel> Delete(decimal id);
    public Task<PatientDomainModel> Block(decimal id);
    public Task<PatientDomainModel> Unblock(decimal id);
    public Task<IEnumerable<PatientDomainModel>> GetBlockedPatients();
    public Task<PatientDomainModel> GetWithMedicalRecord(decimal id);
    public Task<IEnumerable<PatientDomainModel>> ReadAll();
}