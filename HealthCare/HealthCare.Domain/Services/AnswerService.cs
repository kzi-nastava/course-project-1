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
    public class AnswerService : IAnswerService
    {
        private IAnswerRepository _answerRepository;
        public AnswerService(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<IEnumerable<AnswerDomainModel>> GetAll()
        {
            var answers = await _answerRepository.GetAll();
            return parseToModel(answers);
        }

        private IEnumerable<AnswerDomainModel> parseToModel(IEnumerable<Answer> answers)
        {
            List<AnswerDomainModel> answerModels = new List<AnswerDomainModel>();
            foreach (var answer in answers)
            {
                answerModels.Add(parseToModel(answer));
            }
            return answerModels;
        }

        private AnswerDomainModel parseToModel(Answer answer)
        {
            return new AnswerDomainModel
            {
                Id = answer.Id,
                AnswerText  = answer.AnswerText,
                Evaluation = answer.Evaluation,
                DoctorId = answer.DoctorId,
                PatientId  = answer.PatientId,
                QuestionId = answer.QuestionId
            };
        }
    }
}
