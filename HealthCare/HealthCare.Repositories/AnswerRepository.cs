using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Repositories
{
    public interface IAnswerRepository : IRepository<Answer>
    {

    }
    public class AnswerRepository : IAnswerRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public AnswerRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Answer>> GetAll()
        {
            return await _healthCareContext.Answers.ToListAsync();
        }

        public async Task<IEnumerable<Answer>> Get(Doctor doctor)
        {
            return await _healthCareContext.Answers.Where(a => a.DoctorId == doctor.Id).ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
