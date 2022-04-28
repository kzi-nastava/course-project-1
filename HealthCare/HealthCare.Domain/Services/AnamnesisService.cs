using System.Data;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class AnamnesisService : IAnamnesisService
{
    private IAnamnesisRepository _anamnesisRepository;
    private IExaminationRepository _examinationRepository;

    public AnamnesisService(IAnamnesisRepository anamnesisRepository, IExaminationRepository examinationRepository) {
        _anamnesisRepository = anamnesisRepository;
        _examinationRepository = examinationRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<AnamnesisDomainModel>> GetAll()
    {
        var data = await _anamnesisRepository.GetAll();
        if (data == null)
            return null;

        List<AnamnesisDomainModel> results = new List<AnamnesisDomainModel>();
        AnamnesisDomainModel anamnesisModel;
        foreach (var item in data)
        {
            anamnesisModel = new AnamnesisDomainModel
            {
                Id = item.Id,
                Description = item.Description,
                ExaminationId = item.ExaminationId,
                IsDeleted = item.IsDeleted
            };
            results.Add(anamnesisModel);
        }

        return results;
    }
    
    public async Task<IEnumerable<AnamnesisDomainModel>> ReadAll()
    {
        IEnumerable<AnamnesisDomainModel> anamnesis = await GetAll();
        List<AnamnesisDomainModel> result = new List<AnamnesisDomainModel>();
        foreach (var item in anamnesis)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }

    public async Task<AnamnesisDomainModel> Create(CreateAnamnesisDomainModel createModel)
    {
        Anamnesis anamesis = new Anamnesis
        {
            ExaminationId = createModel.ExaminationId,
            IsDeleted = false,
            Description = createModel.Description
        };
        _ = _anamnesisRepository.Post(anamesis);
        _anamnesisRepository.Save();

        return parseToModel(anamesis);
    }

    public AnamnesisDomainModel parseToModel(Anamnesis anamnesis)
    {
        AnamnesisDomainModel model = new AnamnesisDomainModel
        {
            Id = anamnesis.Id,
            Description = anamnesis.Description,
            ExaminationId = anamnesis.ExaminationId,
            IsDeleted = anamnesis.IsDeleted,
        };

        return model;
    }
}