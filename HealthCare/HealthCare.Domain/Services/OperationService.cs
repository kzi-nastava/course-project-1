using HealthCare.Data.Entities;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.OpenApi.Any;

namespace HealthCare.Domain.Interfaces;

public class OperationService : IOperationService 
{
    private IOperationRepository _operationRepository;
    private IRoomRepository _roomRepository;
    private IExaminationRepository _examinationRepository;

    public OperationService(IOperationRepository operationRepository, 
                            IRoomRepository roomRepository, 
                            IExaminationRepository examinationRepository) 
    {
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _examinationRepository = examinationRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<OperationDomainModel>> ReadAll()
    {
        IEnumerable<OperationDomainModel> operations = await GetAll();
        List<OperationDomainModel> result = new List<OperationDomainModel>();
        foreach (OperationDomainModel item in operations)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }
    public async Task<IEnumerable<OperationDomainModel>> GetAll()
    {
        IEnumerable<Operation> data = await _operationRepository.GetAll();
        if (data == null)
            return null;

        List<OperationDomainModel> results = new List<OperationDomainModel>();
        OperationDomainModel operationModel;
        foreach (Operation item in data)
        {
            operationModel = new OperationDomainModel
            {
                IsDeleted = item.IsDeleted,
                PatientId = item.PatientId,
                DoctorId = item.DoctorId,
                Duration = item.Duration,
                StartTime = item.StartTime,
                RoomId = item.RoomId
            };
            results.Add(operationModel);
        }

        return results;
    }

    public async Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id)
    {
        IEnumerable<Operation> data = await _operationRepository.GetAllByDoctorId(id);
        if (data == null)
            return null;

        List<OperationDomainModel> results = new List<OperationDomainModel>();
        foreach (Operation item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }


    private async Task<decimal> GetAvailableRoomId(OperationDomainModel operationModel)
    {
        IEnumerable<Room> rooms = await _roomRepository.GetAllAppointmentRooms("operation");
        foreach (Room room in rooms)
        {
            bool isRoomAvailable = true;
            IEnumerable<Operation> operations = await _operationRepository.GetAllByRoomId(room.Id);
            foreach (Operation operation in operations)
            {
                double difference = (operationModel.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -(double)operationModel.Duration)
                {
                    isRoomAvailable = false;
                    break;
                }
            }
            if (isRoomAvailable)
            {
                return room.Id;
            }
        }
        return -1;
    }

    private async Task<bool> IsPatientOnExaminationAsync(OperationDomainModel operationModel)
    {
        IEnumerable<Examination> patientsExaminations = await _examinationRepository.GetAllByPatientId(operationModel.PatientId);
        foreach (Examination examination in patientsExaminations)
        {
            double difference = (operationModel.StartTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -(double)operationModel.Duration)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> IsPatientOnOperationAsync(OperationDomainModel operationModel)
    {
        IEnumerable<Operation> patientsOperations = await _operationRepository.GetAllByPatientId(operationModel.PatientId);
        foreach (Operation operation in patientsOperations)
        {
            if (operation.Id != operationModel.Id)
            {
                double difference = (operationModel.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -(double)operationModel.Duration)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> IsDoctorOnExaminationAsync(OperationDomainModel operationModel)
    {
        IEnumerable<Examination> doctorsExaminations = await _examinationRepository.GetAllByDoctorId(operationModel.DoctorId);
        if (doctorsExaminations == null)
            return false;
        
        foreach (Examination examination in doctorsExaminations)
        {
            double difference = (operationModel.StartTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -(double)operationModel.Duration)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> IsDoctorOnOperationAsync(OperationDomainModel operationModel)
    {
        IEnumerable<Operation> doctorsOperations = await _operationRepository.GetAllByDoctorId(operationModel.DoctorId);
        foreach (Operation operation in doctorsOperations)
        {
            if (operation.Id != operationModel.Id)
            {
                double difference = (operationModel.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -(double)operationModel.Duration)
                {
                    return true;
                }
            }
           
        }
        return false;
    }

    public async Task<bool> IsDoctorAvailable(OperationDomainModel operationModel)
    {
        return !(await IsDoctorOnOperationAsync(operationModel) ||
                 await IsDoctorOnExaminationAsync(operationModel));
    }

    public async Task<bool> IsPatientAvailable(OperationDomainModel operationModel)
    {
        return !(await IsPatientOnOperationAsync(operationModel) ||
                 await IsPatientOnExaminationAsync(operationModel));
    }


    public async Task<OperationDomainModel> Create(OperationDomainModel operationModel)
    {
        bool doctorAvailable = await IsDoctorAvailable(operationModel);
        bool patientAvailable = await IsPatientAvailable(operationModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: throw exception 
            return null;

        decimal roomId = await GetAvailableRoomId(operationModel);
        if (roomId == -1)
            return null;

        Operation newOperation = new Operation
        {
            PatientId = operationModel.PatientId,
            RoomId = roomId,
            DoctorId = operationModel.DoctorId,
            StartTime = operationModel.StartTime,
            Duration = operationModel.Duration,
            IsDeleted = false
        };

        _ = _operationRepository.Post(newOperation);
        _operationRepository.Save();

        return operationModel;
    }

    public async Task<OperationDomainModel> Update(OperationDomainModel operationModel)
    {
        Operation operation = await _operationRepository.GetById(operationModel.Id);

        if (operation == null)
            return null;

        // to be able to use the validation of availability methods:
        bool doctorAvailable = await IsDoctorAvailable(operationModel);
        bool patientAvailable = await IsPatientAvailable(operationModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: throw exception 
            return null;

        decimal roomId = await GetAvailableRoomId(operationModel);
        if (roomId == -1)
        {
            return null;
        }

        operation.PatientId = operationModel.PatientId;
        operation.DoctorId = operationModel.DoctorId;
        operation.Duration = operationModel.Duration;
        operation.StartTime = operationModel.StartTime;

        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return operationModel;
    }

    public async Task<OperationDomainModel> Delete(OperationDomainModel operationModel)
    {
        Operation operation = await _operationRepository.GetById(operationModel.Id);

        if (operation == null)
            return null;
        
        // logical delete
        operation.IsDeleted = true;
        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return parseToModel(operation);
    }

    private OperationDomainModel parseToModel(Operation operation)
    {
        OperationDomainModel operationModel = new OperationDomainModel
        {
            Id = operation.Id,
            StartTime = operation.StartTime,
            Duration = operation.Duration,
            RoomId = operation.RoomId,
            DoctorId = operation.DoctorId,
            PatientId = operation.PatientId,
            IsDeleted = operation.IsDeleted
        };

        return operationModel;
    }

}