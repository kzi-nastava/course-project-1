using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationApprovalService : IExaminationApprovalService{
    private IExaminationApprovalRepository _examinationApprovalRepository;
    private IExaminationRepository _examinationRepository;

    public ExaminationApprovalService(IExaminationApprovalRepository examinationApprovalRepository, IExaminationRepository examinationRepository) {
        _examinationApprovalRepository = examinationApprovalRepository;
        _examinationRepository = examinationRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<ExaminationApprovalDomainModel>> GetAll()
    {
        var data = await _examinationApprovalRepository.GetAll();
        if (data == null)
            return null;

        List<ExaminationApprovalDomainModel> results = new List<ExaminationApprovalDomainModel>();
        ExaminationApprovalDomainModel examinationApprovalModel;
        foreach (var item in data)
        {
            examinationApprovalModel = new ExaminationApprovalDomainModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                State = item.State,
                NewExaminationId = item.NewExaminationId,
                OldExaminationId = item.OldExaminationId
            };
            //if (item.Examination != null)
            //    examinationApprovalModel.Examination = new ExaminationDomainModel {
            //        doctorId = item.Examination.doctorId,
            //        roomId = item.Examination.roomId,
            //        patientId = item.Examination.patientId,
            //        StartTime = item.Examination.StartTime,
            //        IsDeleted = item.Examination.IsDeleted
            //    };
            results.Add(examinationApprovalModel);
        }

        return results;
    }
    public async Task<IEnumerable<ExaminationApprovalDomainModel>> ReadAll()
    {
        IEnumerable<ExaminationApprovalDomainModel> examinationApprovals = await GetAll();
        List<ExaminationApprovalDomainModel> result = new List<ExaminationApprovalDomainModel>();
        foreach (var item in examinationApprovals)
        {
            if (!item.IsDeleted) result.Add(item);
        }

        return result;
    }

    public async Task<ExaminationApprovalDomainModel> Reject(ExaminationApprovalDomainModel examinationApprovalModel)
    {
        if (!examinationApprovalModel.State.Equals("created")) return null;
        ExaminationApproval examinationApproval = await _examinationApprovalRepository.GetExaminationApprovalById(examinationApprovalModel.Id);
        examinationApproval.State = "rejected";
        _ = _examinationApprovalRepository.Update(examinationApproval);
        _examinationApprovalRepository.Save();
        examinationApprovalModel.State = "rejected";
        return examinationApprovalModel;
    }

    public async Task<ExaminationApprovalDomainModel> Approve(ExaminationApprovalDomainModel examinationApprovalModel)
    {
        if (!examinationApprovalModel.State.Equals("created")) return null;
        
        ExaminationApproval examinationApproval = await _examinationApprovalRepository.GetExaminationApprovalById(examinationApprovalModel.Id);
        examinationApproval.State = "approved";
        _ = _examinationApprovalRepository.Update(examinationApproval);
        _examinationApprovalRepository.Save();
        examinationApprovalModel.State = "approved";

        Examination oldExamination = await _examinationRepository.GetExamination(examinationApprovalModel.OldExaminationId);
        
        if (examinationApproval.OldExaminationId != examinationApproval.NewExaminationId)
        {
            Examination newExamination = await _examinationRepository.GetExamination(examinationApprovalModel.NewExaminationId);
            newExamination.IsDeleted = false;
            _ = _examinationRepository.Update(newExamination);
        }
        // Delete request
        oldExamination.IsDeleted = true;
        _ = _examinationRepository.Update(oldExamination);
        _examinationRepository.Save();
        
        return examinationApprovalModel;
    }
}