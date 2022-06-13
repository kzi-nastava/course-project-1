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
    public class QuestionService : IQuestionService
    {
        private IQuestionRepository _questionRepository;
        private IAnswerRepository _answerRepository;
        public QuestionService(IQuestionRepository questionRepository, IAnswerRepository answerRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<IEnumerable<QuestionDomainModel>> GetAll()
        {
            IEnumerable<Question> questions = await _questionRepository.GetAll();
            IEnumerable<QuestionDomainModel> questionModels = parseToModel(questions);
            
            return questionModels;
        }

        public async Task<IEnumerable<AverageCountEvaluationDomainModel>> GetAverageCountEvaluations()
        {
            IEnumerable<AverageCountEvaluation> averageCountEvaluations =
                await _answerRepository.GetAverageCountEvaluations();
            return parseToModel(averageCountEvaluations);
        }

        private IEnumerable<AverageCountEvaluationDomainModel> parseToModel(IEnumerable<AverageCountEvaluation> averageCountEvaluations)
        {
            List<AverageCountEvaluationDomainModel> models = new List<AverageCountEvaluationDomainModel>();
            foreach (var item in averageCountEvaluations)
            {
                models.Add(parseToModel(item));
            }
            return models;
        }

        private AverageCountEvaluationDomainModel parseToModel(AverageCountEvaluation item)
        {
            return new AverageCountEvaluationDomainModel
            {
                Average = item.Average,
                Count = item.Count,
                Question = item.Question,
            };
        }

        private IEnumerable<QuestionDomainModel> parseToModel(IEnumerable<Question> questions)
        {
            List<QuestionDomainModel> questionModels = new List<QuestionDomainModel>();
            foreach(Question question in questions)
            {
                questionModels.Add(parseToModel(question));
            }
            return questionModels;
        }

        private QuestionDomainModel parseToModel(Question question)
        {
            return new QuestionDomainModel
            {
                Id = question.Id,
                Text = question.Text,
                IsForDoctor = question.IsForDoctor,
            };
        }
    }
}
