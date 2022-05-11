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



        public RenovationService(IJoinRenovationRepository joinRenovationRepository,
            ISplitRenovationRepository splitRenovationRepository,
            ISimpleRenovationRepository simpleRenovationRepository,
            IRoomRepository roomRepository)
        {
            _joinRenovationRepository = joinRenovationRepository;
            _splitRenovationRepository = splitRenovationRepository;
            _simpleRenovationRepository = simpleRenovationRepository;
            _roomRepository = roomRepository;
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

        private async Task<bool> validateSimpleRenovation(SimpleRenovationDomainModel renovation)
        {
            if (renovation.StartDate >= renovation.EndDate)
                throw new Exception("Start is equal or after end");
            Room room = await _roomRepository.GetRoomById(renovation.RoomId);
            if (room == null)
                throw new Exception("Non existant room");

            if (!isAvaliable(room, renovation).Result)
                throw new Exception("Room is already renovating in that period");
            return true;
        }

        private async Task<bool> validateJoinRenovation(JoinRenovationDomainModel renovation)
        {
            if (renovation.StartDate >= renovation.EndDate)
                throw new Exception("Start is equal or after end");
            Room join1 = await _roomRepository.GetRoomById(renovation.JoinRoomId1);
            Room join2 = await _roomRepository.GetRoomById(renovation.JoinRoomId2);
            if (join1 == null || join2 == null)
                throw new Exception("Non existant room");

            if (!isAvaliable(join1, renovation).Result || !isAvaliable(join2, renovation).Result)
                throw new Exception("Room is already renovating in that period");
            return true;
        }

        private async Task<bool> validateSplitRenovation(SplitRenovationDomainModel renovation)
        {
            if (renovation.StartDate >= renovation.EndDate)
                throw new Exception("Start is equal or after end");
            Room split = await _roomRepository.GetRoomById(renovation.SplitRoomId);
            if (split == null)
                throw new Exception("Non existant room");

            if (!isAvaliable(split, renovation).Result)
                throw new Exception("Room is already renovating in that period");
            return true;
        }


        private async Task<bool> isAvaliable(Room room, RenovationDomainModel renovationToCheck)
        {
            IEnumerable<Renovation> roomRenovations = await GetRenovation(room);
            foreach(Renovation renovation in roomRenovations)
            {
                Console.WriteLine(MaxDate(renovation.StartDate, renovationToCheck.StartDate) < MinDate(renovation.EndDate, renovationToCheck.EndDate));
                if (IsDateTimeOverlap(renovation.StartDate, renovation.EndDate,
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

        public async Task<JoinRenovationDomainModel> Create(JoinRenovationDomainModel newRenovationModel)
        {
            if (validateJoinRenovation(newRenovationModel).Result)
            {
                JoinRenovation newJoinRenovation = new JoinRenovation
                {
                    Id = newRenovationModel.Id,
                    EndDate = newRenovationModel.EndDate,
                    StartDate = newRenovationModel.StartDate,
                    JoinRoomId1 = newRenovationModel.JoinRoomId1,
                    JoinRoomId2 = newRenovationModel.JoinRoomId2,
                    ResultRoomId = newRenovationModel.ResultRoomId,
                };
                _joinRenovationRepository.Post(newJoinRenovation);
                _joinRenovationRepository.Save();
                return parseToModel(newJoinRenovation);
            }
            return null;
        }

        public async Task<SplitRenovationDomainModel> Create(SplitRenovationDomainModel newRenovationModel)
        {
            if (validateSplitRenovation(newRenovationModel).Result)
            {
                SplitRenovation newRenovation = new SplitRenovation
                {
                    Id = newRenovationModel.Id,
                    EndDate = newRenovationModel.EndDate,
                    StartDate = newRenovationModel.StartDate,
                    ResultRoomId1 = newRenovationModel.ResultRoomId1,
                    ResultRoomId2 = newRenovationModel.ResultRoomId2,
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
