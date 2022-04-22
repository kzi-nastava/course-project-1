using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForUpdate;

namespace HealthCare.Domain.Interfaces;

public interface IExaminationService : IService<ExaminationDomainModel> {
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForPatient(decimal id);
    public Task<ExaminationDomainModel> Delete(UpdateExaminationDomainModel updateExamination);

}