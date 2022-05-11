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

        public RenovationService(IJoinRenovationRepository joinRenovationRepository,
            ISplitRenovationRepository splitRenovationRepository,
            ISimpleRenovationRepository simpleRenovationRepository)
        {
            _joinRenovationRepository = joinRenovationRepository;
            _splitRenovationRepository = splitRenovationRepository;
            _simpleRenovationRepository = simpleRenovationRepository;
        }

        public async Task<SimpleRenovationDomainModel> Create(SimpleRenovationDomainModel newSimpleRenovationModel)
        {
            //add validation
            SimpleRenovation newSimpleRenovation = new SimpleRenovation
            {
                Id = newSimpleRenovationModel.Id,
                StartDate = newSimpleRenovationModel.StartDate,
                EndDate = newSimpleRenovationModel.EndDate
            };
            _simpleRenovationRepository.Post(newSimpleRenovation);
            _simpleRenovationRepository.Save();
            return parseToModel(newSimpleRenovation);
        }

        public async Task<JoinRenovationDomainModel> Create(JoinRenovationDomainModel newRenovationModel)
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

        public async Task<SplitRenovationDomainModel> Create(SplitRenovationDomainModel newRenovationModel)
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

        

        public async Task<IEnumerable<RenovationDomainModel>> GetAll()
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

            return parseToModel(result);
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
