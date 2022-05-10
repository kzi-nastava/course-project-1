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
        private readonly IRenovationRepository _renovationRepository;

        public async Task<RenovationDomainModel> Create(RenovationDomainModel newRenovationModel)
        {
            //add validation
            Renovation newRenovation = new Renovation
            {
                Id = newRenovationModel.Id,
                StartDate = newRenovationModel.StartDate,
                EndDate = newRenovationModel.EndDate,
                Participant = newRenovationModel.Participant,
                Participant1 = newRenovationModel.Participant1,
                Participant2 = newRenovationModel.Participant2,
            };
            _renovationRepository.Post(newRenovation);
            _renovationRepository.Save();
            return parseToModel(newRenovation);
        }

        public async Task<IEnumerable<RenovationDomainModel>> GetAll()
        {
            IEnumerable<Renovation> renovations = await _renovationRepository.GetAll();
            if(renovations == null) 
                return new List<RenovationDomainModel>();

            return parseToModel(renovations);
        }

        private IEnumerable<RenovationDomainModel> parseToModel(IEnumerable<Renovation> renovations)
        {
            List<RenovationDomainModel> renovationModels = new List<RenovationDomainModel>();
            foreach(Renovation renovation in renovations)
            {
                renovationModels.Add(parseToModel(renovation));
            }
            return renovationModels;
        }

        private RenovationDomainModel parseToModel(Renovation renovation)
        {
            return new RenovationDomainModel
            {
                Id = renovation.Id,
                StartDate = renovation.StartDate,
                EndDate = renovation.EndDate,
                Participant = renovation.Participant,
                Participant1 = renovation.Participant1,
                Participant2 = renovation.Participant2,
            };
        }
    }
}
