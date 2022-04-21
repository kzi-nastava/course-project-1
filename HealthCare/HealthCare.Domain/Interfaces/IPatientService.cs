using HealthCare.Data.Entities;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IPatientService : IService<PatientDomainModel>
{
    public Task<PatientDomainModel> Add(PatientDomainModel patientModel);
    public Task<PatientDomainModel> Update(PatientDomainModel patientModel);
    public Task<PatientDomainModel> Delete(PatientDomainModel patientModel);
}