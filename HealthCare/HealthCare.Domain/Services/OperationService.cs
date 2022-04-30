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
    private IPatientRepository _patientRepository;

    public OperationService(IOperationRepository operationRepository, 
                            IRoomRepository roomRepository, 
                            IExaminationRepository examinationRepository,
                            IPatientRepository patientRepository) 
    {
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _examinationRepository = examinationRepository;
        _patientRepository = patientRepository;
    }

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
            return new List<OperationDomainModel>();
        List<OperationDomainModel> results = new List<OperationDomainModel>();
        OperationDomainModel operationModel;
        foreach (Operation item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id)
    {
        IEnumerable<Operation> data = await _operationRepository.GetAllByDoctorId(id);
        if (data == null)
            throw new DataIsNullException();

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

    private async Task<bool> isPatientOnExamination(OperationDomainModel operationModel)
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

    private async Task<bool> isPatientOnOperation(OperationDomainModel operationModel)
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

    private async Task<bool> isDoctorOnExamination(OperationDomainModel operationModel)
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

    private async Task<bool> isDoctorOnOperation(OperationDomainModel operationModel)
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

    private async Task<bool> isDoctorAvailable(OperationDomainModel operationModel)
    {
        return !(await isDoctorOnOperation(operationModel) ||
                 await isDoctorOnExamination(operationModel));
    }

    private async Task<bool> isPatientAvailable(OperationDomainModel operationModel)
    {
        return !(await isPatientOnOperation(operationModel) ||
                 await isPatientOnExamination(operationModel));
    }

    private DateTime removeSeconds(DateTime dateTime)
    {
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;
        int hour = dateTime.Hour;
        int minute = dateTime.Minute;
        int second = 0;
        return new DateTime(year, month, day, hour, minute, second);
    }

    private async Task<bool> isPatientBlocked(decimal patientId)
    {
        Patient patient = await _patientRepository.GetPatientById(patientId);
        if (patient.BlockedBy != null && !patient.BlockedBy.Equals(""))
            return true;

        return false;
    }

    private async Task validateUserInput(OperationDomainModel operationModel)
    {
        if (operationModel.StartTime <= DateTime.UtcNow)
            throw new DateInPastExeption();
        if (await isPatientBlocked(operationModel.PatientId))
            throw new PatientIsBlockedException();
        bool doctorAvailable = await isDoctorAvailable(operationModel);
        bool patientAvailable = await isPatientAvailable(operationModel);
        if (!doctorAvailable)
            throw new DoctorNotAvailableException();
        if (!patientAvailable)
            throw new PatientNotAvailableException();
    }

    public async Task<OperationDomainModel> Create(OperationDomainModel operationModel)
    {
        await validateUserInput(operationModel);

        decimal roomId = await GetAvailableRoomId(operationModel);
        if (roomId == -1)
            throw new NoFreeRoomsException();

        Operation newOperation = new Operation
        {
            PatientId = operationModel.PatientId,
            RoomId = roomId,
            DoctorId = operationModel.DoctorId,
            StartTime = removeSeconds(operationModel.StartTime),
            Duration = operationModel.Duration,
            IsDeleted = false
        };

        _ = _operationRepository.Post(newOperation);
        _operationRepository.Save();

        return parseToModel(newOperation);
    }

    public async Task<OperationDomainModel> Update(OperationDomainModel operationModel)
    {
        await validateUserInput(operationModel);

        Operation operation = await _operationRepository.GetById(operationModel.Id);

        if (operation == null)
            throw new DataIsNullException();

        decimal roomId = await GetAvailableRoomId(operationModel);
        if (roomId == -1)
            throw new NoFreeRoomsException();

        operation.PatientId = operationModel.PatientId;
        operation.DoctorId = operationModel.DoctorId;
        operation.Duration = operationModel.Duration;
        operation.StartTime = removeSeconds(operationModel.StartTime);

        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return parseToModel(operation);
    }

    public async Task<OperationDomainModel> Delete(OperationDomainModel operationModel)
    {
        Operation operation = await _operationRepository.GetById(operationModel.Id);
        
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