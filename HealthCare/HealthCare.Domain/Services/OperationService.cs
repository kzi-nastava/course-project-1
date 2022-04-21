using HealthCare.Domain.Models;
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
                Patient = item.Patient,
                PatientId = item.PatientId,
                Doctor = item.Doctor,
                DoctorId = item.DoctorId,
                Duration = item.Duration,
                Room = item.Room,
                RoomId = item.RoomId
            };
            results.Add(operationModel);
        }

        return results;
    } 
}