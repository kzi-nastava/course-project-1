using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class RenovationService : IRenovationService
    {
        private readonly IJoinRenovationRepository _joinRenovationRepository;
        private readonly ISplitRenovationRepository _splitRenovationRepository;
        private readonly ISimpleRenovationRepository _simpleRenovationRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IExaminationRepository _examinationRepository;
        private readonly IOperationRepository _operationRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IEquipmentRepository _equipmentRepository;


        public RenovationService(IJoinRenovationRepository joinRenovationRepository,
            ISplitRenovationRepository splitRenovationRepository,
            ISimpleRenovationRepository simpleRenovationRepository,
            IRoomRepository roomRepository,
            IRoomTypeRepository roomTypeRepository,
            IExaminationRepository examinationRepository,
            IOperationRepository operationRepository,
            IInventoryRepository inventoryRepository,
            IEquipmentRepository equipmentRepository)
        {
            _joinRenovationRepository = joinRenovationRepository;
            _splitRenovationRepository = splitRenovationRepository;
            _simpleRenovationRepository = simpleRenovationRepository;
            _roomRepository = roomRepository;
            _roomTypeRepository = roomTypeRepository;
            _examinationRepository = examinationRepository;
            _operationRepository = operationRepository;
            _inventoryRepository = inventoryRepository;
            _equipmentRepository = equipmentRepository;
        }


        public async Task<IEnumerable<RenovationDomainModel>> GetAll()
        {
            IEnumerable<Renovation> renovations = await GetAllRenovations();
            return parseToModel(renovations);
        }

        public async Task<IEnumerable<JoinRenovationDomainModel>> GetJoin()
        {
            IEnumerable<JoinRenovation> joinRenovations = await _joinRenovationRepository.GetAll();
            return parseToModel(joinRenovations);
        }

        public async Task<IEnumerable<SplitRenovationDomainModel>> GetSplit()
        {
            IEnumerable<SplitRenovation> splitRenovations = await _splitRenovationRepository.GetAll();
            return parseToModel(splitRenovations);
        }

        public async Task<IEnumerable<SimpleRenovationDomainModel>> GetSimple()
        {
            IEnumerable<SimpleRenovation> simpleRenovations = await _simpleRenovationRepository.GetAll();
            return parseToModel(simpleRenovations);
        }

        public async Task<IEnumerable<Renovation>> GetRenovation(Room room)
        {
            IEnumerable<Renovation> renovations = await GetAllRenovations();
            return renovations.Where(r => room.Id == room.Id);
        }

        private async Task<IEnumerable<Examination>> GetExaminations(Room room)
        {
            IEnumerable<Examination> examinations = await _examinationRepository.GetAll();
            return examinations.Where(e => e.RoomId == room.Id);
        }

        private async Task<IEnumerable<Operation>> GetOperations(Room room)
        {
            IEnumerable<Operation> operations = await _operationRepository.GetAll();
            return operations.Where(o => o.RoomId == room.Id);
        }




        private async Task<bool> validateSimpleRenovation(CreateSimpleRenovationDTO renovation)
        {
            if (renovation.StartDate >= renovation.EndDate)
                throw new Exception("Start is equal or after end");
            Room room = await _roomRepository.GetRoomById(renovation.RoomId);
            if (room == null)
                throw new Exception("Non existant room");

            if (!IsAvaliable(room, renovation).Result)
                throw new Exception("Room is already renovating in that period");
            return true;
        }

        private async Task<bool> validateJoinRenovation(CreateJoinRenovationDTO dto)
        {
            if (dto.StartDate >= dto.EndDate)
                throw new Exception("Start is equal or after end");

            if (dto.resultRoomName.Trim().Equals(String.Empty))
                throw new Exception("Invalid room name given");

            Room join1 = await _roomRepository.GetRoomById(dto.JoinRoomId1);
            Room join2 = await _roomRepository.GetRoomById(dto.JoinRoomId2);
            if (join1 == null || join2 == null)
                throw new Exception("Non existant room");

            RoomType roomType = await _roomTypeRepository.GetById(dto.roomTypeId);
            if (roomType == null)
                throw new Exception("Non existant room type");

            if (!IsAvaliable(join1, dto).Result || !IsAvaliable(join2, dto).Result)
                throw new Exception("Room is already renovating in that period");

            return true;
        }

        private async Task<bool> validateSplitRenovation(CreateSplitRenovationDTO dto)
        {
            if (dto.resultRoomName1.Trim().Equals(String.Empty) || dto.resultRoomName2.Trim().Equals(String.Empty))
                throw new Exception("Invalid room name given");

            RoomType roomType1 = await _roomTypeRepository.GetById(dto.roomTypeId1);
            RoomType roomType2 = await _roomTypeRepository.GetById(dto.roomTypeId2);
            if (roomType1 == null || roomType2 == null)
                throw new Exception("No room type with such id exists");

            if (dto.StartDate >= dto.EndDate)
                throw new Exception("Start is equal or after end");
            Room split = await _roomRepository.GetRoomById(dto.SplitRoomId);
            if (split == null)
                throw new Exception("Non existant room");

            if (!IsAvaliable(split, dto).Result)
                throw new Exception("Room is already renovating in that period");
            return true;
        }


        private async Task<bool> IsAvaliable(Room room, CreateRenovationDTO renovationToCheck)
        {
            IEnumerable<Renovation> roomRenovations = await GetRenovation(room);
            foreach(Renovation renovation in roomRenovations)
            {
                if (IsDateTimeOverlap(renovation.StartDate, renovation.EndDate,
                    renovationToCheck.StartDate, renovationToCheck.EndDate))
                    return false;
            }

            IEnumerable<Examination> roomExaminations = await GetExaminations(room);
            foreach (Examination examination in roomExaminations)
            {
                if (IsDateTimeOverlap(examination.StartTime, examination.StartTime.AddMinutes(15),
                    renovationToCheck.StartDate, renovationToCheck.EndDate))
                    return false;
            }

            IEnumerable<Operation> roomOperations = await GetOperations(room);
            foreach (Operation operation in roomOperations)
            {
                if (IsDateTimeOverlap(operation.StartTime, operation.StartTime.AddMinutes(Decimal.ToDouble(operation.Duration)),
                    renovationToCheck.StartDate, renovationToCheck.EndDate))
                    return false;
            }

            return true;
        }

       

        private bool IsDateTimeOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return MaxDate(start1, start2) < MinDate(end1, end2);

        }

        private DateTime MaxDate(DateTime time1, DateTime time2)
        {
            return (time1 > time2 ? time1 : time2);
        }

        private DateTime MinDate(DateTime time1, DateTime time2)
        {
            return (time1 < time2 ? time1 : time2);
        }


        public async Task<SimpleRenovationDomainModel> Create(CreateSimpleRenovationDTO dto)
        {
            if (validateSimpleRenovation(dto).Result)
            {
                SimpleRenovation newSimpleRenovation = new SimpleRenovation
                {
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    RoomId = dto.RoomId
                };
                _simpleRenovationRepository.Post(newSimpleRenovation);
                _simpleRenovationRepository.Save();
                return parseToModel(newSimpleRenovation);
            }
            return null;

        }

        public async Task ExecuteComplexRenovations()
        {
            await ExecuteJoinRenovations();
            await ExecuteSplitRenovations();
        }

        public async Task ExecuteSplitRenovations()
        {
            IEnumerable<SplitRenovation> renovations = await _splitRenovationRepository.GetAll();
            foreach(SplitRenovation renovation in renovations)
            {
                Room splitRoom = await _roomRepository.GetRoomById(renovation.SplitRoomId);
                Room resultRoom1 = await _roomRepository.GetRoomById(renovation.ResultRoomId1);
                Room resultRoom2 = await _roomRepository.GetRoomById(renovation.ResultRoomId2);
                if(renovation.EndDate < DateTime.Now && !resultRoom1.IsFormed && !resultRoom2.IsFormed)
                {
                    splitRoom.IsDeleted = true;
                    await TransferEquipmentToStorage(splitRoom);
                    resultRoom1.IsFormed = true;
                    resultRoom2.IsFormed = true;

                    _roomRepository.Update(splitRoom);
                    _roomRepository.Update(resultRoom1);
                    _roomRepository.Update(resultRoom2);
                    _roomRepository.Save();
                }
            }
        }

        public async Task ExecuteJoinRenovations()
        {
            IEnumerable<JoinRenovation> renovations = await _joinRenovationRepository.GetAll();
            foreach (JoinRenovation renovation in renovations)
            {
                Room joinRoom1 = await _roomRepository.GetRoomById(renovation.JoinRoomId1);
                Room joinRoom2 = await _roomRepository.GetRoomById(renovation.JoinRoomId2);
                Room resultRoom = await _roomRepository.GetRoomById(renovation.ResultRoomId);
                if (renovation.EndDate < DateTime.Now && !resultRoom.IsFormed)
                {
                    joinRoom1.IsDeleted = true;
                    joinRoom2.IsDeleted = true;
                    await TransferEquipmentToStorage(joinRoom1);
                    await TransferEquipmentToStorage(joinRoom2);
                    resultRoom.IsFormed = true;
                    
                    _roomRepository.Update(joinRoom1);
                    _roomRepository.Update(joinRoom2);
                    _roomRepository.Update(resultRoom);
                    _roomRepository.Save();
                }
            }
        }

        private async Task TransferEquipmentToStorage(Room room)
        {
            // get all inventories that posses room equipment
            IEnumerable<Inventory> roomInventories = await _inventoryRepository.Get(room);

            // get storage room
            Room storageRoom = await _roomRepository.GetRoomByName("storage");
            if (storageRoom == null)
                throw new Exception("No storage in system");
            
            // move all equipment to storage room
            foreach(Inventory roomInventory in roomInventories)
            {
                Equipment equipment = await _equipmentRepository.GetById(roomInventory.EquipmentId);
                Inventory storageRoomInventory = await _inventoryRepository.Get(storageRoom, equipment);

                // make new inventory if it doesn't exist
                if(storageRoomInventory == null)
                {
                    storageRoomInventory = new Inventory
                    {
                        RoomId = storageRoom.Id,
                        EquipmentId = equipment.Id,
                        Amount = roomInventory.Amount,
                        IsDeleted = false,
                    };
                    _inventoryRepository.Post(storageRoomInventory);
                }
                else
                {
                    storageRoomInventory.Amount += roomInventory.Amount;
                    _inventoryRepository.Update(storageRoomInventory);
                }
                roomInventory.Amount = 0;
                _inventoryRepository.Update(roomInventory);   
                _inventoryRepository.Save();
                

            }
        }

        public async Task<JoinRenovationDomainModel> Create(CreateJoinRenovationDTO dto)
        {
            if (validateJoinRenovation(dto).Result)
            {

                Room result = new Room
                {
                    RoomName = dto.resultRoomName,
                    RoomTypeId = dto.roomTypeId,
                    IsDeleted = false,
                    IsFormed = false
                };

                _roomRepository.Post(result);
                _roomRepository.Save();

                JoinRenovation newJoinRenovation = new JoinRenovation
                {
                    EndDate = dto.EndDate,
                    StartDate = dto.StartDate,
                    JoinRoomId1 = dto.JoinRoomId1,
                    JoinRoomId2 = dto.JoinRoomId2,
                    ResultRoomId = result.Id,
                };
                _joinRenovationRepository.Post(newJoinRenovation);
                _joinRenovationRepository.Save();
                return parseToModel(newJoinRenovation);
            }
            return null;
        }

        public async Task<SplitRenovationDomainModel> Create(CreateSplitRenovationDTO dto)
        {
            if (validateSplitRenovation(dto).Result)
            {

                Room result1 = new Room
                {
                    RoomName = dto.resultRoomName1,
                    RoomTypeId = dto.roomTypeId1,
                    IsDeleted = false,
                    IsFormed = false,
                };
                _roomRepository.Post(result1);

                Room result2 = new Room
                {
                    RoomName = dto.resultRoomName2,
                    RoomTypeId = dto.roomTypeId2,
                    IsDeleted = false,
                    IsFormed = false,
                };
                _roomRepository.Post(result2);
                _roomRepository.Save();

                SplitRenovation newRenovation = new SplitRenovation
                {
                    EndDate = dto.EndDate,
                    StartDate = dto.StartDate,
                    ResultRoomId1 = result1.Id,
                    ResultRoomId2 = result2.Id,
                    SplitRoomId = dto.SplitRoomId,
                };
                _splitRenovationRepository.Post(newRenovation);
                _splitRenovationRepository.Save();
                return parseToModel(newRenovation);
            }
            return null;
        }




        public async Task<IEnumerable<Renovation>> GetAllRenovations()
        {
            IEnumerable<Renovation> simpleRenovations = await _simpleRenovationRepository.GetAll();
            IEnumerable<Renovation> joinRenovations = await _joinRenovationRepository.GetAll();
            IEnumerable<Renovation> splitRenovations = await _splitRenovationRepository.GetAll();

            IEnumerable<Renovation> result = new List<Renovation>();

            if (simpleRenovations != null)
                result = result.Concat<Renovation>(simpleRenovations);
            if (joinRenovations != null)
                result = result.Concat<Renovation>(joinRenovations);
            if (splitRenovations != null)
                result = result.Concat<Renovation>(splitRenovations);

            return result;
        }

        





        public SimpleRenovationDomainModel parseToModel(SimpleRenovation simpleRenovation)
        {
            return new SimpleRenovationDomainModel
            {
                Id = simpleRenovation.Id,
                EndDate = simpleRenovation.EndDate,
                StartDate = simpleRenovation.StartDate,
                RoomId = simpleRenovation.RoomId
            };
        }

        public JoinRenovationDomainModel parseToModel(JoinRenovation joinRenovation)
        {
            return new JoinRenovationDomainModel
            {
                Id = joinRenovation.Id,
                EndDate = joinRenovation.EndDate,
                StartDate = joinRenovation.StartDate,
                JoinRoomId1 = joinRenovation.JoinRoomId1,
                JoinRoomId2 = joinRenovation.JoinRoomId2,
                ResultRoomId = joinRenovation.ResultRoomId
            };
        }

        public SplitRenovationDomainModel parseToModel(SplitRenovation splitRenovation)
        {
            return new SplitRenovationDomainModel
            {
                Id = splitRenovation.Id,
                EndDate = splitRenovation.EndDate,
                StartDate = splitRenovation.StartDate,
                ResultRoomId1 = splitRenovation.ResultRoomId1,
                ResultRoomId2 = splitRenovation.ResultRoomId2,
                SplitRoomId = splitRenovation.SplitRoomId
            };
        }
        public RenovationDomainModel parseToModel(Renovation renovation)
        {
            return new RenovationDomainModel
            {
                Id = renovation.Id,
                EndDate = renovation.EndDate,
                StartDate = renovation.StartDate,
            };
        }

        private IEnumerable<RenovationDomainModel> parseToModel(IEnumerable<Renovation> renovations)
        {
            List<RenovationDomainModel> renovationModels = new List<RenovationDomainModel>();
            foreach (Renovation renovation in renovations)
            {
                renovationModels.Add(parseToModel(renovation));
            }
            return renovationModels;
        }
        public IEnumerable<JoinRenovationDomainModel> parseToModel(IEnumerable<JoinRenovation> renovations)
        {
            List<JoinRenovationDomainModel> renovationModels = new List<JoinRenovationDomainModel>();
            foreach (var renovation in renovations)
            {
                renovationModels.Add(parseToModel(renovation));
            }
            return renovationModels;
        }

        public IEnumerable<SplitRenovationDomainModel> parseToModel(IEnumerable<SplitRenovation> renovations)
        {
            List<SplitRenovationDomainModel> renovationModels = new List<SplitRenovationDomainModel>();
            foreach (var renovation in renovations)
            {
                renovationModels.Add(parseToModel(renovation));
            }
            return renovationModels;
        }

        public IEnumerable<SimpleRenovationDomainModel> parseToModel(IEnumerable<SimpleRenovation> renovations)
        {
            List<SimpleRenovationDomainModel> renovationModels = new List<SimpleRenovationDomainModel>();
            foreach (var renovation in renovations)
            {
                renovationModels.Add(parseToModel(renovation));
            }
            return renovationModels;
        }

        
    }
}
