using Magellan.DataAccess.Interfaces;
using Magellan.Domain;

namespace Magellan.DataAccess;

public class ModelRepository : IModelRepository
{
    private readonly List<ModelEntity> _models = new()
    {
        new() { Id = "gpt-3", Name = "GPT-3", Index = 1 },
        new() { Id = "gpt-4", Name = "GPT-4", Index = 2 }
    };
    
    public IEnumerable<ModelEntity> GetModels()
    {
        return _models;
    }

    public bool ModelExists(string modelId) 
        => _models.Any(x => x.Id == modelId);
}