using HealthCare.Data.Entities;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForUpdate;
using HealthCare.Repositories;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.OpenApi.Any;

namespace HealthCare.Domain.Interfaces;

public class OperationService : IOperationService {
    private IOperationRepository _operationRepository;
    private IRoomRepository _roomRepository;
    private IExaminationRepository _examinationRepository;

    public OperationService(IOperationRepository operationRepository, IRoomRepository roomRepository, IExaminationRepository examinationRepository) {
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
        foreach (var item in operations)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }
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
                IsDeleted = item.IsDeleted,
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


    private async Task<decimal> GetAvailableRoomId(CreateOperationDomainModel operationModel)
    {
        var rooms = await _roomRepository.GetAllAppointmentRooms("operation");
        foreach (Room room in rooms)
        {
            bool isRoomAvailable = true;
            var operations = await _operationRepository.GetAllByRoomId(room.Id);
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

    private async Task<bool> IsPatientOnExaminationAsync(CreateOperationDomainModel operationModel)
    {
        var patientsExaminations = await _examinationRepository.GetAllByPatientId(operationModel.PatientId);
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

    private async Task<bool> IsPatientOnOperationAsync(CreateOperationDomainModel operationModel)
    {
        var patientsOperations = await _operationRepository.GetAllByPatientId(operationModel.PatientId);
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

    private async Task<bool> IsDoctorOnExaminationAsync(CreateOperationDomainModel operationModel)
    {
        var doctorsExaminations = await _examinationRepository.GetAllByDoctorId(operationModel.DoctorId);
        if (doctorsExaminations == null)
        {
            return false;
        }
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

    private async Task<bool> IsDoctorOnOperationAsync(CreateOperationDomainModel operationModel)
    {
        var doctorsOperations = await _operationRepository.GetAllByDoctorId(operationModel.DoctorId);
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

    public async Task<bool> IsDoctorAvailable(CreateOperationDomainModel operationModel)
    {
        bool isDoctorAvailable = true;
        if (await IsDoctorOnExaminationAsync(operationModel))
            isDoctorAvailable = false;
        if (await IsDoctorOnOperationAsync(operationModel))
            isDoctorAvailable = false;

        if (!isDoctorAvailable)
        {
            //TODO: Think about the return value if doctor is not available
            return false;
        }

        return true;
    }

    public async Task<bool> IsPatientAvailable(CreateOperationDomainModel operationModel)
    {
        bool isPatientAvailable = true;
        if (await IsPatientOnExaminationAsync(operationModel))
            isPatientAvailable = false;
        if (await IsPatientOnOperationAsync(operationModel))
            isPatientAvailable = false;

        if (!isPatientAvailable)
        {
            return false;
        }

        return true;
    }


    public async Task<CreateOperationDomainModel> Create(CreateOperationDomainModel operationModel)
    {

        bool doctorAvailable = await IsDoctorAvailable(operationModel);
        bool patientAvailable = await IsPatientAvailable(operationModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: Think about the return value if doctor is not available
            return null;

        decimal roomId = await GetAvailableRoomId(operationModel);
        if (roomId == -1)
        {
            return null;
        }

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

    public async Task<UpdateOperationDomainModel> Update(UpdateOperationDomainModel operationModel)
    {
        var operation = await _operationRepository.GetById(operationModel.OldOperationId);

        if (operation == null)
        {
            return null;
        }

        // to be able to use the validation of availability methods:
        CreateOperationDomainModel createOperationDomainModel = new CreateOperationDomainModel
        {
            Id = operationModel.OldOperationId,
            DoctorId = operationModel.NewDoctorId,
            PatientId = operationModel.NewPatientId,
            StartTime = operationModel.NewStartTime,
            Duration = operationModel.NewDuration
        };

        bool doctorAvailable = await IsDoctorAvailable(createOperationDomainModel);
        bool patientAvailable = await IsPatientAvailable(createOperationDomainModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: Think about the return value if doctor is not available
            return null;

        decimal roomId = await GetAvailableRoomId(createOperationDomainModel);
        if (roomId == -1)
        {
            return null;
        }

        operation.PatientId = operationModel.NewPatientId;
        operation.DoctorId = operationModel.NewDoctorId;
        operation.Duration = operationModel.NewDuration;
        operation.StartTime = operationModel.NewStartTime;

        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return operationModel;
    }

    public async Task<OperationDomainModel> Delete(decimal id)
    {
        var operation = await _operationRepository.GetById(id);

        if (operation == null)
        {
            return null;
        }

        // logical delete
        operation.IsDeleted = true;
        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return parseToModel(operation);
    }

    private OperationDomainModel parseToModel(Operation operation)
    {
        OperationDomainModel examinationModel = new OperationDomainModel
        {
            Id = operation.Id,
            StartTime = operation.StartTime,
            Duration = operation.Duration,
            RoomId = operation.RoomId,
            DoctorId = operation.DoctorId,
            PatientId = operation.PatientId,
            IsDeleted = operation.IsDeleted
        };

        return examinationModel;
    }

}