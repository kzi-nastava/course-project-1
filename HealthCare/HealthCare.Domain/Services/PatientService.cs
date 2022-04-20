using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class PatientService : IPatientService{
    private IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository) {
        _patientRepository = patientRepository;
    }

    public Task<IEnumerable<PatientDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}