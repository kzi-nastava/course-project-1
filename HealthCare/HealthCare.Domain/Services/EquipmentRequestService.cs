using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentRequestService : IEquipmentRequestService
{
    private readonly IEquipmentRequestRepository _equipmentRequestRepository;
    
    public EquipmentRequestService(IEquipmentRequestRepository equipmentRequestRepository)
    {
        _equipmentRequestRepository = equipmentRequestRepository;
    }

    public async Task<IEnumerable<EquipmentRequestDomainModel>> GetAll()
    {
        IEnumerable<EquipmentRequest> equipmentRequests = await _equipmentRequestRepository.GetAll();
        if (equipmentRequests == null)
            throw new DataIsNullException();

        List<EquipmentRequestDomainModel> results = new List<EquipmentRequestDomainModel>();
        foreach (EquipmentRequest item in equipmentRequests)
            results.Add(ParseToModel(item));

        return results;
    }

    public static EquipmentRequestDomainModel ParseToModel(EquipmentRequest equipmentRequest)
    {
        EquipmentRequestDomainModel equipmentRequestModel = new EquipmentRequestDomainModel 
        {
            Id = equipmentRequest.Id,
            EquipmentId = equipmentRequest.EquipmentId,
            Amount = equipmentRequest.Amount,
            ExecutionTime = equipmentRequest.ExecutionTime,
            IsExecuted = equipmentRequest.IsExecuted
        };
        
        if (equipmentRequest.Equipment != null)
            equipmentRequestModel.Equipment = EquipmentService.ParseToModel(equipmentRequest.Equipment);
        
        return equipmentRequestModel;
    }
    
    public static EquipmentRequest ParseFromModel(EquipmentRequestDomainModel equipmentRequestModel)
    {
        EquipmentRequest equipmentRequest = new EquipmentRequest 
        {
            Id = equipmentRequestModel.Id,
            EquipmentId = equipmentRequestModel.EquipmentId,
            Amount = equipmentRequestModel.Amount,
            ExecutionTime = equipmentRequestModel.ExecutionTime,
            IsExecuted = equipmentRequestModel.IsExecuted
        };
        
        if (equipmentRequestModel.Equipment != null)
            equipmentRequest.Equipment = EquipmentService.ParseFromModel(equipmentRequestModel.Equipment);
        
        return equipmentRequest;
    }
}
