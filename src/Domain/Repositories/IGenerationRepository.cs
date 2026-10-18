namespace Domain.Repositories;

public interface IGenerationRepository
{
    Task Save(Generation generation);

    Task<Generation?> GetById(string id);
}
