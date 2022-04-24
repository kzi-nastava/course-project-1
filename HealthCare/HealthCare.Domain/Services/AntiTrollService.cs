using System.Data;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class AntiTrollService : IAntiTrollService {
    private IAntiTrollRepository _antiTrollRepository;

    public AntiTrollService(IAntiTrollRepository antiTrollRepository) {
        _antiTrollRepository = antiTrollRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *

    private AntiTrollDomainModel parseToDomainModel(AntiTroll antiTroll) {
        AntiTrollDomainModel antiTrollModel = new AntiTrollDomainModel {
            Id = antiTroll.Id,
            PatientId = antiTroll.PatientId,
            DateCreated = antiTroll.DateCreated,
            State = antiTroll.State
        };
        return antiTrollModel;
    }

    public async Task<IEnumerable<AntiTrollDomainModel>> GetAll() {
        var data = await _antiTrollRepository.GetAll();
        if (data == null)
            return null;

        List<AntiTrollDomainModel> results = new List<AntiTrollDomainModel>();
        AntiTrollDomainModel antiTrollModel;
        foreach (var item in data) {
            results.Add(parseToDomainModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<AntiTrollDomainModel>> GetByPatientId(decimal patientId) {
        var data = await _antiTrollRepository.GetByPatientId(patientId);
        if (data == null)
            return null;

        List<AntiTrollDomainModel> results = new List<AntiTrollDomainModel>();
        AntiTrollDomainModel antiTrollModel;
        foreach (var item in data) {
            results.Add(parseToDomainModel(item));
        }

        return results;
    }
}