using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IExaminationApprovalService : IService<ExaminationApprovalDomainModel> 
{
    public Task<ExaminationApprovalDomainModel> Reject(ExaminationApprovalDomainModel examinationModel);
    public Task<ExaminationApprovalDomainModel> Approve(ExaminationApprovalDomainModel examinationModel);
    public Task<IEnumerable<ExaminationApprovalDomainModel>> ReadAll();
}