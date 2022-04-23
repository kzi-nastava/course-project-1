using HealthCare.Data.Entities;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Repositories;

namespace HealthCare.Domain.Interfaces;

public class OperationService : IOperationService{
    private IOperationRepository _operationRepository;

    public OperationService(IOperationRepository operationRepository) {
        _operationRepository = operationRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<OperationDomainModel>> GetAll()
    {
        var data = await _operationRepository.GetAll();
        if (data == null)
            return null;

        List<OperationDomainModel> results = new List<OperationDomainModel>();
        OperationDomainModel operationModel;
        foreach (var item in data)
        {
            operationModel = new OperationDomainModel
            {
                isDeleted = item.isDeleted,
                //Patient = item.Patient,
                PatientId = item.PatientId,
                //Doctor = item.Doctor,
                DoctorId = item.DoctorId,
                Duration = item.Duration,
                StartTime = item.StartTime,
                //Room = item.Room,
                RoomId = item.RoomId
            };
            results.Add(operationModel);
        }

        return results;
    }

    public async Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id)
    {
        var data = await _operationRepository.GetAllByDoctorId(id);
        if (data == null)
            return null;

        List<OperationDomainModel> results = new List<OperationDomainModel>();

        foreach (var item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<CreateOperationDomainModel> Create(CreateOperationDomainModel operationModel)
    {

        bool doctorAvailable = await IsDoctorAvailable(examinationModel);
        bool patientAvailable = await IsPatientAvailable(examinationModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: Think about the return value if doctor is not available
            return null;

        decimal roomId = await GetAvailableRoomId(examinationModel);
        if (roomId == -1)
        {
            return null;
        }

        Examination newExamination = new Examination
        {
            patientId = examinationModel.patientId,
            roomId = roomId,
            doctorId = examinationModel.doctorId,
            StartTime = examinationModel.StartTime,
            IsDeleted = false,
            Anamnesis = null,
            ExaminationApproval = null
        };

        _ = _examinationRepository.Post(newExamination);
        _examinationRepository.Save();

        return examinationModel;
    }

    private OperationDomainModel parseToModel(Operation operation)
    {
        OperationDomainModel examinationModel = new OperationDomainModel
        {
            StartTime = operation.StartTime,
            Duration = operation.Duration,
            RoomId = operation.RoomId,
            DoctorId = operation.DoctorId,
            PatientId = operation.PatientId,
            isDeleted = operation.isDeleted
        };

        return examinationModel;
    }
}