using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForUpdate;

namespace HealthCare.Domain.Interfaces;

public interface IExaminationService : IService<ExaminationDomainModel> {
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForPatient(decimal id);
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForDoctor(decimal id);
    public Task<ExaminationDomainModel> Delete(DeleteExaminationDomainModel deleteExaminationModel, bool writeToAntiTroll);
    public Task<CreateExaminationDomainModel> Create(CreateExaminationDomainModel createExaminationModel, bool writeToAntiTroll);
    public Task<UpdateExaminationDomainModel> Update(UpdateExaminationDomainModel updateExaminationModel);
    public Task<IEnumerable<ExaminationDomainModel>> ReadAll();
}