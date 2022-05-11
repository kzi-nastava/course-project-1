using HealthCare.Data.Entities;
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


        public RenovationService(IJoinRenovationRepository joinRenovationRepository,
            ISplitRenovationRepository splitRenovationRepository,
            ISimpleRenovationRepository simpleRenovationRepository,
            IRoomRepository roomRepository,
            IRoomTypeRepository roomTypeRepository,
            IExaminationRepository examinationRepository,
            IOperationRepository operationRepository)
        {
            _joinRenovationRepository = joinRenovationRepository;
            _splitRenovationRepository = splitRenovationRepository;
            _simpleRenovationRepository = simpleRenovationRepository;
            _roomRepository = roomRepository;
            _roomTypeRepository = roomTypeRepository;
            _examinationRepository = examinationRepository;
            _operationRepository = operationRepository;
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

        public async Task<IEnumerable<Renovation>> GetRenovation(Room r)
        {
            IEnumerable<Renovation> renovations = await GetAllRenovations();
            return renovations.Where(r => r.Id == r.Id);
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




        private async Task<bool> validateSimpleRenovation(SimpleRenovationDomainModel renovation)
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

        private async Task<bool> validateJoinRenovation(JoinRenovationDomainModel renovation, string resultRoomName, decimal roomTypeId)
        {
            if (renovation.StartDate >= renovation.EndDate)
                throw new Exception("Start is equal or after end");

            if (resultRoomName.Trim().Equals(String.Empty))
                throw new Exception("Invalid room name given");

            Room join1 = await _roomRepository.GetRoomById(renovation.JoinRoomId1);
            Room join2 = await _roomRepository.GetRoomById(renovation.JoinRoomId2);
            if (join1 == null || join2 == null)
                throw new Exception("Non existant room");

            RoomType roomType = await _roomTypeRepository.GetById(roomTypeId);
            if (roomType == null)
                throw new Exception("Non existant room type");

            if (!IsAvaliable(join1, renovation).Result || !IsAvaliable(join2, renovation).Result)
                throw new Exception("Room is already renovating in that period");

            return true;
        }

        private async Task<bool> validateSplitRenovation(SplitRenovationDomainModel renovation,
            string resultRoomName1, string resultRoomName2,
            decimal roomTypeId1, decimal roomTypeId2)
        {
            if (resultRoomName1.Trim().Equals(String.Empty) || resultRoomName2.Trim().Equals(String.Empty))
                throw new Exception("Invalid room name given");

            RoomType roomType1 = await _roomTypeRepository.GetById(roomTypeId1);
            RoomType roomType2 = await _roomTypeRepository.GetById(roomTypeId2);
            if (roomType1 == null || roomType2 == null)
                throw new Exception("No room type with such id exists");

            if (renovation.StartDate >= renovation.EndDate)
                throw new Exception("Start is equal or after end");
            Room split = await _roomRepository.GetRoomById(renovation.SplitRoomId);
            if (split == null)
                throw new Exception("Non existant room");

            if (!IsAvaliable(split, renovation).Result)
                throw new Exception("Room is already renovating in that period");
            return true;
        }


        private async Task<bool> IsAvaliable(Room room, RenovationDomainModel renovationToCheck)
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


        public async Task<SimpleRenovationDomainModel> Create(SimpleRenovationDomainModel newSimpleRenovationModel)
        {
            if (validateSimpleRenovation(newSimpleRenovationModel).Result)
            {
                SimpleRenovation newSimpleRenovation = new SimpleRenovation
                {
                    Id = newSimpleRenovationModel.Id,
                    StartDate = newSimpleRenovationModel.StartDate,
                    EndDate = newSimpleRenovationModel.EndDate,
                    RoomId = newSimpleRenovationModel.RoomId
                };
                _simpleRenovationRepository.Post(newSimpleRenovation);
                _simpleRenovationRepository.Save();
                return parseToModel(newSimpleRenovation);
            }
            return null;

        }

        public async Task<JoinRenovationDomainModel> Create(JoinRenovationDomainModel newRenovationModel,
            string resultRoomName, decimal roomTypeId)
        {
            if (validateJoinRenovation(newRenovationModel, resultRoomName, roomTypeId).Result)
            {
                Room joinRoom1 = await _roomRepository.GetRoomById(newRenovationModel.JoinRoomId1);
                joinRoom1.IsDeleted = true;

                Room joinRoom2 = await _roomRepository.GetRoomById(newRenovationModel.JoinRoomId2);
                joinRoom2.IsDeleted = true;

                Room result = new Room
                {
                    RoomName = resultRoomName,
                    RoomTypeId = roomTypeId,
                    IsDeleted = false,
                };

                _roomRepository.Post(result);
                _roomRepository.Update(joinRoom1);
                _roomRepository.Update(joinRoom2);
                _roomRepository.Save();

                //divide equipment here


                JoinRenovation newJoinRenovation = new JoinRenovation
                {
                    Id = newRenovationModel.Id,
                    EndDate = newRenovationModel.EndDate,
                    StartDate = newRenovationModel.StartDate,
                    JoinRoomId1 = newRenovationModel.JoinRoomId1,
                    JoinRoomId2 = newRenovationModel.JoinRoomId2,
                    ResultRoomId = result.Id,
                };
                _joinRenovationRepository.Post(newJoinRenovation);
                _joinRenovationRepository.Save();
                return parseToModel(newJoinRenovation);
            }
            return null;
        }

        public async Task<SplitRenovationDomainModel> Create(SplitRenovationDomainModel newRenovationModel,
            string resultRoomName1, string resultRoomName2, decimal roomTypeId1, decimal roomTypeId2)
        {
            if (validateSplitRenovation(newRenovationModel, resultRoomName1, resultRoomName2, roomTypeId1, roomTypeId2).Result)
            {
                Room split = await _roomRepository.GetRoomById(newRenovationModel.SplitRoomId);
                split.IsDeleted = true;
                _roomRepository.Update(split);

                //divide equipment here

                Room result1 = new Room
                {
                    RoomName = resultRoomName1,
                    RoomTypeId = roomTypeId1,
                    IsDeleted = false,
                };
                _roomRepository.Post(result1);

                Room result2 = new Room
                {
                    RoomName = resultRoomName2,
                    RoomTypeId = roomTypeId2,
                    IsDeleted = false,
                };
                _roomRepository.Post(result2);
                _roomRepository.Save();

                SplitRenovation newRenovation = new SplitRenovation
                {
                    Id = newRenovationModel.Id,
                    EndDate = newRenovationModel.EndDate,
                    StartDate = newRenovationModel.StartDate,
                    ResultRoomId1 = result1.Id,
                    ResultRoomId2 = result2.Id,
                    SplitRoomId = newRenovationModel.SplitRoomId,
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



        public IEnumerable<RenovationDomainModel> parseToModel(List<Renovation> renovations)
        {
            List<RenovationDomainModel> renovationModels = new List<RenovationDomainModel>();
            foreach (var renovation in renovations)
            {
                renovationModels.Add(parseToModel(renovation));
            }
            return renovationModels;
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
