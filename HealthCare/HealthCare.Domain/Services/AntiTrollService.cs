using System.Data;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class AntiTrollService : IAntiTrollService 
{
    private IAntiTrollRepository _antiTrollRepository;

    public AntiTrollService(IAntiTrollRepository antiTrollRepository) 
    {
        _antiTrollRepository = antiTrollRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *

    private AntiTrollDomainModel parseToModel(AntiTroll antiTroll) 
    {
        AntiTrollDomainModel antiTrollModel = new AntiTrollDomainModel 
        {
            Id = antiTroll.Id,
            PatientId = antiTroll.PatientId,
            DateCreated = antiTroll.DateCreated,
            State = antiTroll.State
        };
        return antiTrollModel;
    }

    public async Task<IEnumerable<AntiTrollDomainModel>> GetAll() 
    {
        IEnumerable<AntiTroll> data = await _antiTrollRepository.GetAll();
        if (data == null)
            return null;

        List<AntiTrollDomainModel> results = new List<AntiTrollDomainModel>();
        foreach (AntiTroll item in data) 
        {
            results.Add(parseToModel(item));
        }
        return results;
    }

    public async Task<IEnumerable<AntiTrollDomainModel>> GetByPatientId(decimal patientId) 
    {
        IEnumerable<AntiTroll> data = await _antiTrollRepository.GetByPatientId(patientId);
        if (data == null)
            return null;

        List<AntiTrollDomainModel> results = new List<AntiTrollDomainModel>();
        foreach (AntiTroll item in data) 
        {
            results.Add(parseToModel(item));
        }

        return results;
    }
}