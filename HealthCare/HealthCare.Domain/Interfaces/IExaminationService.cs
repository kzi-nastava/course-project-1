using HealthCare.Domain.DataTransferObjects;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IExaminationService : IService<ExaminationDomainModel> 
{
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForPatient(decimal id);
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForDoctor(decimal id);
    public Task<ExaminationDomainModel> Delete(ExaminationDomainModel examinationModel, bool isPatient);
    public Task<ExaminationDomainModel> Create(ExaminationDomainModel examinationModel, bool isPatient);
    public Task<ExaminationDomainModel> Update(ExaminationDomainModel examinationModel, bool isPatient);

    public Task<IEnumerable<ExaminationDomainModel>> GetRecommendedExaminations(ParamsForRecommendingFreeExaminationsDTO paramsDTO)
    public Task<IEnumerable<ExaminationDomainModel>> ReadAll();
}