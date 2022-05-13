using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IExaminationService : IService<ExaminationDomainModel> 
{
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForPatient(decimal id);
    public Task<IEnumerable<ExaminationDomainModel>> GetAllForPatientSorted(SortExaminationDTO dto, IDoctorService doctorService);

    public Task<IEnumerable<ExaminationDomainModel>> GetAllForDoctor(decimal id);
    public Task<ExaminationDomainModel> Delete(DeleteExaminationDTO dto);
    public Task<ExaminationDomainModel> Create(CUExaminationDTO dto);
    public Task<ExaminationDomainModel> Update(CUExaminationDTO dto);

    public Task<IEnumerable<CUExaminationDTO>> GetRecommendedExaminations(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService);
    public Task<IEnumerable<ExaminationDomainModel>> ReadAll();
    public Task<IEnumerable<ExaminationDomainModel>> SearchByAnamnesis(SearchByNameDTO dto);

    public Task<IEnumerable<ExaminationDomainModel>> CreateUrgent(CreateUrgentExaminationDTO dto, IDoctorService doctorService, IPatientService patientService);
}