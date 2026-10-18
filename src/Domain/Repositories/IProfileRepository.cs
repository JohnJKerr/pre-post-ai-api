namespace Domain.Repositories;

public interface IProfileRepository
{
    Task Save(Profile profile);
    Task<Profile?> GetByHandle(string handle);
}
