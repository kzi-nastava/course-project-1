using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationApprovalService : IExaminationApprovalService{
    private IExaminationApprovalRepository _examinationApprovalRepository;

    public ExaminationApprovalService(IExaminationApprovalRepository examinationApprovalRepository) {
        _examinationApprovalRepository = examinationApprovalRepository;
    }

    public Task<IEnumerable<ExaminationApprovalDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }    
}