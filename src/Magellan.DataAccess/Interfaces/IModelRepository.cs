using Magellan.Domain;

namespace Magellan.DataAccess.Interfaces;

public interface IModelRepository
{
    public IEnumerable<ModelEntity> GetModels();
    
    public bool ModelExists(string modelId);
}