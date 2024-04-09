using Magellan.DataAccess;

namespace Magellan.UnitTests;

public class ModelRepositoryTest
{
    private readonly ModelRepository _repository = new();

    [Fact]
    public void AllModelsHaveDifferentId()
    {
        var models = _repository.GetModels().ToList();
        Assert.True(models.Select(x => x.Id).Distinct().Count() == models.Count);
    }
    
    [Fact]
    public void AllModelsHaveDifferentName()
    {
        var models = _repository.GetModels().ToList();
        Assert.True(models.Select(x => x.Name).Distinct().Count() == models.Count);
    }
    
    [Fact]
    public void AllModelsHaveDifferentIndex()
    {
        var models = _repository.GetModels().ToList();
        Assert.True(models.Select(x => x.Index).Distinct().Count() == models.Count);
    }
}