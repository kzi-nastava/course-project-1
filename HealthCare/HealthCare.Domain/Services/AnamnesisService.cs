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

    public AnamnesisService(IAnamnesisRepository anamnesisRepository) {
        _anamnesisRepository = anamnesisRepository;
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
                isDeleted = item.isDeleted
            };
            results.Add(anamnesisModel);
        }

        return results;
    }

    public async Task<AnamnesisDomainModel> Create(AnamnesisDomainModel createModel)
    {
        Anamnesis anamesis = new Anamnesis
        {
            ExaminationId = createModel.ExaminationId,
            isDeleted = false,
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
            isDeleted = anamnesis.isDeleted
        };

        return model;
    }
}