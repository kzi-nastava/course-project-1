using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationService : IExaminationService{
    private IExaminationRepository _examinationRepository;

    public ExaminationService(IExaminationRepository examinationRepository) {
        _examinationRepository = examinationRepository;
    }

    public Task<IEnumerable<ExaminationDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}